using System;
using System.Text.Json;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Azure.Functions.Worker.Extensions.Mcp;
using Microsoft.Extensions.Logging;

namespace agents_function_app01;

public class get_weather_azure_function01
{
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

    private readonly ILogger<get_weather_azure_function01> _logger;

    public get_weather_azure_function01(ILogger<get_weather_azure_function01> logger)
    {
        _logger = logger;
    }




    [Function(nameof(MyCatBornDate))]
    public string MyCatBornDate(
        [McpToolTrigger("MyCatBornDate", "Returns the born date of the cat")] ToolInvocationContext context
    )
    {
        var answer = "Your cat was born on 2020-01-01";
        _logger.LogInformation(answer);
        return answer;
    }


    [Function(nameof(OthersCatBornDate))]
    public string OthersCatBornDate(
        [McpToolTrigger("OthersCatBornDate", "Returns the born date of anyone's cat, given its name.")] ToolInvocationContext context,
        [McpToolProperty("CatName", "string", "The name of the cat.")] string CatName
    )
    {
        _logger.LogInformation($"C# Queue trigger function processed: {CatName}");
        var yearBorn = DateTime.UtcNow.Year - CatName.Length;
        var dayBorn = DateTime.UtcNow.Day;
        var monthBorn = DateTime.UtcNow.Month;
        var answer = $"The cat named {CatName} was born on {yearBorn}-{monthBorn:D2}-{dayBorn:D2}";
        _logger.LogInformation(answer);
        return answer;
    }

    [Function(nameof(get_weather_azure_function01))]
    public string Run(
        [McpToolTrigger("get_weather_azure_function01", "Given a location, retrieves the weather information.")] ToolInvocationContext context,
        [McpToolProperty("Location", "string", "The name of the location.")] string Location
        )
    {
        _logger.LogInformation($"C# Queue trigger function processed: {Location.ToUpper()}");

        // extract the json parts from the Body
        var weatherRequest = JsonSerializer.Deserialize<WeatherRequest>(Location);

        // Generate the weather response and serialize it to JSON for output
        var weatherResponse = GenerateWeatherResponse(Location);
        return JsonSerializer.Serialize(weatherResponse);
    }
}