import os, asyncio
from autogen_agentchat.agents import AssistantAgent
from autogen_agentchat.agents import CodeExecutorAgent
from autogen_agentchat.teams import RoundRobinGroupChat
from autogen_agentchat.conditions import TextMentionTermination, MaxMessageTermination
from autogen_ext.models.openai import AzureOpenAIChatCompletionClient
from autogen_ext.code_executors.local import LocalCommandLineCodeExecutor
from autogen_agentchat.ui import Console


def config_list_from_json(env_or_file, filter_dict):
    import json
    with open(env_or_file, 'r') as file:
        data = json.load(file)
    
    filtered_data = [
        item for item in data
        if item.get('endpoint') == filter_dict.get('endpoint') and item.get('deployment') == filter_dict.get('deployment')
    ]    
    return filtered_data


async def main() -> None:
    env_or_file='./../config/models_list.json'
    filter_dict = {
        'endpoint': 'https://mmoaiswc-01.openai.azure.com/',
        'deployment': 'gpt-4o-2024-08-06'
    }

    model_name =  filter_dict["deployment"]

    autogen_config = config_list_from_json(env_or_file, filter_dict)[0] # we take the first combination of model and endpoint

    # beaware NOT to show the API KEY
    print(f'AutoGen Configuration: {autogen_config["endpoint"]}, {autogen_config["deployment"]}, {autogen_config["api_version"]}, ...') 

    model_client = AzureOpenAIChatCompletionClient(
        azure_endpoint=autogen_config["endpoint"],
        api_key=autogen_config["api_key"],
        model = autogen_config["model"],
        azure_deployment = autogen_config["deployment"],
        api_version=autogen_config["api_version"],
        seed = 41,
        temperature = 0.1,
    )


    # ASSISTANT AGENT
    assistant = AssistantAgent(
        name="assistant",
        model_client=model_client,
        system_message="""
        You are a helpful assistant. Write all code in python. 
        Reply only 'TERMINATE' after you get confirmation that the code was successfully executed.
        """,
    )
    
    # CODE EXECUTOR AGENT
    code_executor = CodeExecutorAgent(
        name="code_executor",
        code_executor=LocalCommandLineCodeExecutor(work_dir="coding"),
    )

    termination = TextMentionTermination("TERMINATE") | MaxMessageTermination(20)

    # The group chat will alternate between the assistant and the code executor.
    group_chat = RoundRobinGroupChat([assistant, code_executor], termination_condition=termination)

    stream = group_chat.run_stream(task="""
        Could you please create a bar chart for the operating profit using 
        the following data and provide the file to me? 
        Company A: $1.2 million, Company B: $2.5 million, Company C: $3.0 million, 
        Company D: $1.8 million
        """
    )

    await Console(stream)
    
    
asyncio.run(main())