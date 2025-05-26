// Copyright (c) Microsoft. All rights reserved.

// Create an agent with Grounding with Bing Search
// docs: https://learn.microsoft.com/en-us/azure/ai-services/agents/quickstart?pivots=programming-language-csharp
// sample: https://learn.microsoft.com/en-us/azure/ai-services/agents/how-to/tools/bing-grounding?tabs=csharp&pivots=code-example
// "dotnet add package Azure.AI.Projects --prerelease" + "dotnet add package Azure.Identity"

using Azure;
using Azure.AI.Projects; // dotnet add package Azure.AI.Projects --prerelease
using Azure.Identity; // dotnet add package Azure.Identity

namespace LLMSettings;

internal class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("Application starts");


        // Step 0: Load configuration from environment variables or user secrets.
        var aiSettings = new AISettings();
        Console.WriteLine($"AZURE_OPENAI_ENDPOINT: {aiSettings.AzureOpenAI.Endpoint}\n" +
        $"AZURE_OPENAI_CHAT_DEPLOYMENT_NAME: {aiSettings.AzureOpenAI.ChatModelDeployment}");


        // Step 1: Create a client object with the connection string to the AI project containing other resources too
        var connectionString = aiSettings.AzureAICONNECTIONSTRING;
        var clientOptions = new AIProjectClientOptions();
        var projectClient = new AIProjectClient(connectionString, new DefaultAzureCredential(), clientOptions);


        // Step 2: Enable the Grounding with Bing search tool
        var bingConnection = await projectClient.GetConnectionsClient().GetConnectionAsync(aiSettings.BINGCONNECTIONNAME);
        var connectionId = bingConnection.Value.Id;
        var connectionList = new ToolConnectionList
        {
            ConnectionList = { new ToolConnection(connectionId) }
        };
        var bingGroundingTool = new BingGroundingToolDefinition(connectionList);


        // Step 3: Create the AI Foundry Agents Client
        AgentsClient agentsClient = projectClient.GetAgentsClient();


        // Step 4: Get or Create the AI Foundry Agent
        string? userInput;
        Response<Azure.AI.Projects.Agent> agentResponse;

        Console.Write("If you have an agent ID, please enter here. Otherwise, leave this blank > ");
        userInput = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(userInput))
        {
            agentResponse = await agentsClient.CreateAgentAsync(
            model: aiSettings.AzureOpenAI.ChatModelDeployment,
            name: "C#-bing-agent",
            instructions: "You are a helpful assistant.",
            tools: new List<ToolDefinition> { bingGroundingTool });
        }
        else
        {
            agentResponse = await agentsClient.GetAgentAsync(assistantId: userInput);
        }
        
        Azure.AI.Projects.Agent ai_agent = agentResponse.Value;


        // Step 5: Chat with the AI Foundry Agent
        await ChatWithAgentAsync(agent:ai_agent, agentsClient:agentsClient);


        // Step 6: Delete the agent        
        await agentsClient.DeleteAgentAsync(agentId: ai_agent.Id);
        Console.WriteLine($"\n\nAgent {ai_agent.Name} (id {ai_agent.Id}) was deleted.");
    }

    private static async Task ChatWithAgentAsync(object agent, AgentsClient? agentsClient = null)
    {
        string? userInput;
        bool exit_chat = false;

        do
        {
            Console.Write("User > ");
            userInput = Console.ReadLine();

            // Check if userInput is not null or EXIT
            exit_chat = string.IsNullOrWhiteSpace(userInput) || userInput.Trim().Equals("EXIT", StringComparison.OrdinalIgnoreCase);

            if (!exit_chat)
            {
                // check if it's an AI Foundry Agent 
                if (agent is Azure.AI.Projects.Agent ai_agent) // debug: (agent.GetType().FullName == "Azure.AI.Projects.Agent")
                {
                    //var ai_agent = (Azure.AI.Projects.Agent)agent;
                    // Create thread for communication
                    Response<AgentThread> threadResponse = await agentsClient.CreateThreadAsync();
                    AgentThread thread = threadResponse.Value;

                    // Create message to thread
                    Response<ThreadMessage> messageResponse = await agentsClient.CreateMessageAsync(
                        threadId: thread.Id,
                        role: MessageRole.User,
                        content: userInput);
                    ThreadMessage message = messageResponse.Value;

                    // Run the agent
                    Response<ThreadRun> runResponse = await agentsClient.CreateRunAsync(thread, ai_agent);
                    do
                    {
                        await Task.Delay(TimeSpan.FromMilliseconds(500));
                        runResponse = await agentsClient.GetRunAsync(thread.Id, runResponse.Value.Id);
                    }
                    while (runResponse.Value.Status == RunStatus.Queued
                        || runResponse.Value.Status == RunStatus.InProgress);

                    Response<PageableList<ThreadMessage>> afterRunMessagesResponse
                        = await agentsClient.GetMessagesAsync(thread.Id);
                    IReadOnlyList<ThreadMessage> messages = afterRunMessagesResponse.Value.Data;

                    // Note: messages iterate from newest to oldest, with the messages[0] being the most recent
                    foreach (ThreadMessage threadMessage in messages.Reverse())
                    {
                        Console.Write($"\n\n{threadMessage.CreatedAt:yyyy-MM-dd HH:mm:ss} - {threadMessage.Role,10}:");
                        foreach (MessageContent contentItem in threadMessage.ContentItems)
                        {
                            if (contentItem is MessageTextContent textItem)
                            {
                                Console.Write($"\n{textItem.Text}");
                            }
                            else if (contentItem is MessageImageFileContent imageFileItem)
                            {
                                Console.Write($"\n<image from ID: {imageFileItem.FileId}");
                            }
                            Console.WriteLine();
                        }
                    }
                }
                // check if it's an ASSISTANT agent
                /*
                if (agent is OpenAIAssistantAgent assistantAgent)
                {
                    List<string> fileIds = [];
                    string threadId = await assistantAgent.CreateThreadAsync();
                    await assistantAgent.AddChatMessageAsync(threadId, new ChatMessageContent(AuthorRole.User, userInput));

                    try
                    {
                        bool isCode = false;
                        await foreach (StreamingChatMessageContent response in assistantAgent.InvokeStreamingAsync(threadId))
                        {
                            if (isCode != (response.Metadata?.ContainsKey(OpenAIAssistantAgent.CodeInterpreterMetadataKey) ?? false))
                            {
                                Console.WriteLine();
                                isCode = !isCode;
                            }
                            // Display response.
                            Console.Write($"{response.Content}");

                            // Capture file IDs for downloading
                            fileIds.AddRange(response.Items.OfType<StreamingFileReferenceContent>().Select(item => item.FileId));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error (but don't worry, we can continue ;-)): {ex.Message}");
                    }

                    fileIds = RemoveDuplicates(fileIds);
                    Console.WriteLine();

                    // Download any images referenced in the response
                    await DownloadResponseImageAsync(fileClient, fileIds);

                    fileIds.Clear();
                }

                // check if it's a CHAT COMPLETION agent
                else if (agent is ChatCompletionAgent chatAgent)
                {
                    var history = new ChatHistory();
                    history.AddUserMessage(userInput);
                    await foreach (ChatMessageContent response in chatAgent.InvokeAsync(history))
                    {
                        Console.WriteLine($"{response.Content}");
                        // Add the message from the agent to the chat history
                        history.AddMessage(response.Role, response.Content ?? string.Empty);
                    }
                }*/

                // check if it's a GROUP CHAT agent
                /*
                else if (agent is AgentGroupChat groupAgent)
                {
                    var author_name = ""; // used in the streaming to check when the author changes
                    List<string> fileIds = [];
                    groupAgent.AddChatMessage(new ChatMessageContent(AuthorRole.User, userInput));
                    await foreach (StreamingChatMessageContent response in groupAgent.InvokeStreamingAsync())
                    //await foreach (ChatMessageContent response in groupAgent.InvokeAsync())
                    {
                        // Add the message from the agent to the chat history
                        //Console.WriteLine($"# {response.Role} - {response.AuthorName ?? "*"}: '{response.Content}'");
                        if (response.AuthorName != author_name)
                        {
                            Console.WriteLine($"\n\n### New turn: {response.Role} - {response.AuthorName ?? "*"}:\n");
                            author_name = response.AuthorName;
                        }
                        Console.Write(response.Content);

                        // Capture file IDs for downloading
                        fileIds.AddRange(response.Items.OfType<StreamingFileReferenceContent>().Select(item => item.FileId));
                    }
                    fileIds = RemoveDuplicates(fileIds);
                    Console.WriteLine();

                    // Download any images referenced in the response
                    await DownloadResponseImageAsync(fileClient, fileIds);

                    fileIds.Clear();
                    Console.WriteLine();
                    exit_chat = exit_chat || groupAgent.IsComplete;
                }*/
            }
        } while (!exit_chat);
    }
}