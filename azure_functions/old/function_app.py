import logging
import azure.functions as func

app = func.FunctionApp()

@app.function_name(name="BlobOutput1")
@app.route(route="file")
@app.blob_input(arg_name="inputblob",
                path="mycontainer/arriving_files/{name}.csv",
                connection="AzureWebJobsStorage")
@app.blob_output(arg_name="outputblob",
                path="mycontainer/arriving_files/test.txt",
                connection="AzureWebJobsStorage")
def main(req: func.HttpRequest, inputblob: str, outputblob: func.Out[str]):
    logging.info(f'Python Queue trigger function processed {len(inputblob)} bytes')
    outputblob.set(inputblob)
    return "ok"