using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;

namespace agents_function_app
{
    public class get_weather_azure_function
    {
        [FunctionName("get_weather_azure_function")]
        public void Run([QueueTrigger("weather-queue", Connection = "AzureWebJobsStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem.ToUpper()}");
        }
    }
}