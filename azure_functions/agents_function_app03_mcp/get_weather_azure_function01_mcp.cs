using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using Microsoft.Extensions.Logging;

namespace agents_function_app03_mcp;

public class get_weather_azure_function_mcp
{
    private readonly ILogger<get_weather_azure_function_mcp> _logger;

    public get_weather_azure_function_mcp(ILogger<get_weather_azure_function_mcp> logger)
    {
        _logger = logger;
    }

    public static string GenerateWeatherResponse(string location)
    {
        // Calculate temperature based on location name length
        int temperature = location.Length;
        string response = $"Temperature is {temperature} Celsius degrees and it is sunny in {location}";

        return response;
    }

    [Function(nameof(get_weather_azure_function_mcp))]
    public string Run(
        [McpToolTrigger("get_weather_mcp", "Gets the weather for a specific location.")] ToolInvocationContext context,
        [McpToolProperty("Location", "string", "The location to check the weather for.")] string Location
        )
    {
        _logger.LogInformation("Context: {Context}", JsonSerializer.Serialize(context));
        _logger.LogInformation("Received Location parameter: '{Location}' (Length: {Length})", Location ?? "NULL", Location?.Length ?? 0);
        
        // Generate the weather response and serialize it to JSON for output
        var safeLocation = Location ?? "Unknown";
        var weatherResponse = GenerateWeatherResponse(safeLocation);
        _logger.LogInformation("Generated weather response: {WeatherResponse}", weatherResponse);

        return weatherResponse;
    }
}