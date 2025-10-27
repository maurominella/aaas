using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace agents_function_app01;

public class get_weather_azure_function01
{
    private readonly ILogger<get_weather_azure_function01> _logger;

    public get_weather_azure_function01(ILogger<get_weather_azure_function01> logger)
    {
        _logger = logger;
    }

    private class WeatherRequest
    {
        public string? Location { get; set; }
        public string? CorrelationId { get; set; }
        public override string ToString()
        {
            return $"Location: {Location}, CorrelationId: {CorrelationId}";
        }
    }

    public class WeatherResponse
    {
        public string? Value { get; set; }
        public string? CorrelationId { get; set; }
        public override string ToString()
        {
            return $"Value: {Value}, CorrelationId: {CorrelationId}";
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
            Value = resultMessage,
            CorrelationId = input.CorrelationId
        };
        return response;
    }

    [Function(nameof(get_weather_azure_function01))]
    [QueueOutput("azure-function-foo-output", Connection = "AzureWebJobsStorage")]
    public string Run([QueueTrigger(
        "azure-function-foo-input", Connection = "AzureWebJobsStorage")] QueueMessage message)
    {
        _logger.LogInformation("C# Queue trigger function processed: {messageText}",
                message.MessageText.ToUpper());

        // extract the json parts from the Body
        var weatherRequest = JsonSerializer.Deserialize<WeatherRequest>(message.MessageText.ToString());
        // Generate the weather response and serialize it to JSON for output
        var weatherResponse = GenerateWeatherResponse(message.MessageText);
        _logger.LogInformation($"Result: {weatherResponse.Value}");

        // Serialize the response to JSON and return as output
        return JsonSerializer.Serialize(weatherResponse);
    }
}