// Copyright (c) Microsoft. All rights reserved.

using System.Reflection;
using DotNetEnv; // dotnet add package DotNetEnv
using Microsoft.Extensions.Configuration;

namespace LLMSettings;
/*
# SETTINGS FOR .NET
# - Azure OpenAI for .NET
AZUREOPENAISETTINGS__ENDPOINT = "<azure-openai-endpoint>"
AZUREOPENAISETTINGS__CHATMODELDEPLOYMENT = "<azure-openai-model>"
AZUREOPENAISETTINGS__APIKEY = "<azure-openai-apikey>"
OPENAISETTINGS__APIVERSION = "<azure-openai-apiversion>"

# - OpenAI settings for .NET
OPENAISETTINGS__APIKEY      = "<openai-apikey>"
OPENAISETTINGS__CHATMODEL   = "<openai-model>"
OPENAISETTINGS__APIVERSION  = "<openai-apiversion>"
*/


public class AISettings
{
    private readonly IConfigurationRoot _configRoot;

    private AzureOpenAISettings _azureOpenAI;
    public AzureOpenAISettings AzureOpenAI => this._azureOpenAI ??= this.GetSettings<AISettings.AzureOpenAISettings>();

    private OpenAISettings _openAI;
    public OpenAISettings OpenAI => this._openAI ??= this.GetSettings<AISettings.OpenAISettings>();

    public string AzureAICONNECTIONSTRING => this._configRoot["PROJECT_CONNECTION_STRING"];
    public string BINGCONNECTIONNAME => this._configRoot["BING_CONNECTION_NAME"];


    public class AzureOpenAISettings
    {
        public string Endpoint { get; set; } = string.Empty;
        public string ChatModelDeployment { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

    public class OpenAISettings
    {
        public string ChatModel { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string ApiVersion { get; set; } = string.Empty;
    }


    public TSettings GetSettings<TSettings>()
    {
        var section = this._configRoot.GetSection(typeof(TSettings).Name);
        if (!section.Exists())
        {
            throw new InvalidOperationException($"Configuration section '{typeof(TSettings).Name}' is missing.");
        }
        return section.Get<TSettings>()!;
    }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public AISettings()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        // Load environment variables from .env file
        Env.Load(this.EnvFilePath());

        // Initialize configRoot
        this._configRoot =
            new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .AddUserSecrets(Assembly.GetExecutingAssembly(), optional: true)
                .Build();

        // Debugging statement to ensure configRoot is initialized
        if (this._configRoot == null)
        {
            throw new InvalidOperationException("Configuration root is not initialized.");
        }

        var azureOpenAISection = this._configRoot.GetSection("AzureOpenAISettings");
    }

    public string EnvFilePath()
    {
        string _projectRoot;
        // Get the base directory
        DirectoryInfo baseDirectory = new(AppDomain.CurrentDomain.BaseDirectory);

        // retrieve the project root folder

        if (baseDirectory.Parent.Parent.Name == "bin")
        {
            _projectRoot = baseDirectory.Parent.Parent.Parent.FullName;
        }
        else
        {
            _projectRoot = baseDirectory.FullName;
        }

        string envFilePath = Path.Combine(_projectRoot, "./../../config/credentials_my.env");

        return envFilePath;
    }

    // Add this method to print configuration
    public void PrintConfiguration()
    {
        foreach (var kvp in this._configRoot.AsEnumerable())
        {
            Console.WriteLine($"{kvp.Key}: {kvp.Value}");
        }
    }

    // Add this method to print configuration
    public void PrintConfigurationSections()
    {
        foreach (var section in this._configRoot.GetChildren())
        {
            Console.WriteLine($"Section: {section.Key}");
            foreach (var child in section.GetChildren())
            {
                Console.WriteLine($"  {child.Key}: {child.Value}");
            }
        }
    }

    public void VerifyEnvironmentVariables()
    {
        Console.WriteLine($"AZURE_OPENAI_ENDPOINT: {this._configRoot["AZURE_OPENAI_ENDPOINT"]}");
        Console.WriteLine($"AZURE_OPENAI_API_KEY: {this._configRoot["AZURE_OPENAI_API_KEY"]}");
        Console.WriteLine($"AZURE_OPENAI_CHAT_DEPLOYMENT_NAME: {this._configRoot["AZURE_OPENAI_CHAT_DEPLOYMENT_NAME"]}");
    }
}