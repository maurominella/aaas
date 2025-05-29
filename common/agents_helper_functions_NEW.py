from azure.ai.projects import AIProjectClient

def list_all_agents (project_client:AIProjectClient, limit:int = 100):
    all_agents = list(project_client.agents.list_agents())[:limit] # Convert iterator to a list
    summary = f'{len(all_agents)} agents'
    return {"summary": summary, "content": all_agents}


def list_all_threads(project_client:AIProjectClient, limit:int = 100):
    all_threads = list(project_client.agents.threads.list())[:limit] # Convert iterator to a list
    summary = f'{len(all_threads)} threads'
    return {"summary": summary, "content": all_threads}


def list_all_files (project_client:AIProjectClient):
    all_files = project_client.agents.files.list()["data"]
    summary = f'{len(all_files)} files' 
    return {"summary": summary, "content": all_files}


def list_all_runs (project_client: AIProjectClient, limit:int = 100):
    all_threads = list(project_client.agents.threads.list())[:limit] # Convert iterator to a list
    all_runs = [
        {"thread_id": thread.id, "runs": list(project_client.agents.runs.list(thread_id=thread.id, limit=limit))}
        for thread in all_threads]
    
    runs_per_thread = [
        {"thread_id": runs_per_thread["thread_id"], "runs_nr": len(runs_per_thread["runs"])}
        for runs_per_thread in all_runs
    ]

    summary = f'{sum(thread["runs_nr"] for thread in runs_per_thread)} runs in {len(all_threads)} threads'
    
    return {"summary": summary, "content": all_runs}


def list_all_runsteps (project_client: AIProjectClient, limit:int = 100):
    all_threads = list_all_threads(project_client)["content"]
    all_runs = list_all_runs(project_client)["content"]

    all_runsteps = [
        {"thread_id": run.thread_id, "run_id": run.id, "run_steps": project_client.agents.list_run_steps(thread_id=run.thread_id, run_id=run.id, limit=limit)}
        for runs_per_thread in all_runs
        for run in runs_per_thread["runs"]["data"]
    ]
    steps_per_thread_and_run= [
        {
            "thread_id": steps_per_thread_and_run["thread_id"],
            "run_id": steps_per_thread_and_run["run_id"], 
            "runsteps_nr": len(steps_per_thread_and_run["run_steps"]["data"])
        }
        for steps_per_thread_and_run in all_runsteps
    ]
    
    summary = f'{sum(entry['runsteps_nr'] for entry in steps_per_thread_and_run)} run steps in {len(steps_per_thread_and_run)} pairs of (thread, run) of project <{project_client.scope["project_name"]}>'
    return {"summary": summary, "content": all_runsteps}


def list_all_messages (project_client: AIProjectClient, limit:int = 100):
    all_threads = list_all_threads(project_client)["content"]
    
    all_messages = [
        {"thread_id": thread.id, "messages": project_client.agents.list_messages(thread_id=thread.id, limit=limit)}
        for thread in all_threads["data"]]
    
    messages_per_thread = [    
        {"thread_id": messages_per_thread["thread_id"], "messages_nr": len(messages_per_thread["messages"]["data"])}
        for messages_per_thread in all_messages
    ]           
    
    summary = f'{sum(thread["messages_nr"] for thread in messages_per_thread)} messages in {len(all_threads["data"])} threads of project <{project_client.scope["project_name"]}>'
    return {"summary": summary, "content": all_messages}


def list_all_vectorstores (project_client:AIProjectClient, limit:int = 100):
    all_vectorstores = list(project_client.agents.vector_stores.list())[:limit] # Convert iterator to a list
    summary = f'{len(all_vectorstores)} vector stores'    
    return {"summary": summary, "content": all_vectorstores}