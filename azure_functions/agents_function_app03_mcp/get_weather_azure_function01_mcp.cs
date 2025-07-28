using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using Microsoft.Extensions.Logging;

namespace agents_function_app03_mcp;

public class get_weather_azure_function01_mcp
{
    private readonly ILogger<get_weather_azure_function01_mcp> _logger;

    public get_weather_azure_function01_mcp(ILogger<get_weather_azure_function01_mcp> logger)
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

    [Function(nameof(get_weather_azure_function01_mcp))]
    public void Run(
        [McpToolTrigger("get_weather_azure_function01_mcp", "Gets the weather for a specific location.")] ToolInvocationContext context,
        [McpToolProperty("Location", "string", "The location to check the weather for.")] string Location
        )
    {
        _logger.LogInformation("Context: {Context}", JsonSerializer.Serialize(context));
        _logger.LogInformation("Received Location parameter: '{Location}' (Length: {Length})", Location ?? "NULL", Location?.Length ?? 0);
        _logger.LogInformation("C# Queue trigger function processed: {Location}", Location);
    }
}