using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
// using DotNetEnv; // dotnet add package DotNetEnv not needed here
using System.Collections;
using System.Text.Json;


namespace agents_function_app01
{
    public class get_weather_azure_function01
    {

        public class WeatherRequest
        {
            public string Location { get; set; }
            public string CorrelationId { get; set; }
        }

        public class WeatherResponse
        {
            public string Result { get; set; }
            public string CorrelationId { get; set; }
        }
        public get_weather_azure_function01()
        {

            IDictionary envVars = Environment.GetEnvironmentVariables();

            // to set up a variable with PowerShell: $env:AzureWebJobsStorage = "DefaultEndpointsProtocol=https;AccountName=..."
            // to set up a variable with bash: export AzureWebJobsStorage="DefaultEndpointsProtocol..."
            // to remove a variable with PowerShell: Remove-Item Env:\AzureWebJobsStorage
            // to remove a variable with bash: unset AzureWebJobsStorage

            foreach (DictionaryEntry entry in envVars)
            {
                string key = entry.Key.ToString();
                string value = entry.Value?.ToString() ?? "<null>";
                // Console.WriteLine($"[INIT ENV] {key} = {value}");
            }
        }

        public static WeatherResponse GenerateWeatherResponse(string jsonString)
        {
            // Deserialize input JSON
            var input = JsonSerializer.Deserialize<WeatherRequest>(jsonString);
            Console.WriteLine($"[GenerateWeatherResponse] input: {input}");

            if (input == null || string.IsNullOrEmpty(input.Location))
                throw new ArgumentException("Invalid input JSON");

            // Calculate temperature based on location name length
            int temperature = input.Location.Length;
            string resultMessage = $"Temperature is {temperature} Celsius degrees and it is sunny in {input.Location}";

            // Create response object
            WeatherResponse response = new()
            {
                Result = resultMessage,
                CorrelationId = input.CorrelationId
            };

            return response;
        }

        [FunctionName("get_weather_azure_function01")]
        public void Run(
            [QueueTrigger("azure-function-foo-input", Connection = "AzureWebJobsStorage")] string InputQueueItem, ILogger log,
            [Queue("azure-function-foo-output", Connection = "AzureWebJobsStorage")] out string OutputQueueItem)
        {

            log.LogInformation($"C# Queue trigger function processed: {InputQueueItem.ToUpper()}");

            var weatherResponse = GenerateWeatherResponse(InputQueueItem);

            log.LogInformation($"Result: {weatherResponse.Result}");

            // Serialize response object to JSON
            OutputQueueItem = JsonSerializer.Serialize(weatherResponse);
        }
    }
}