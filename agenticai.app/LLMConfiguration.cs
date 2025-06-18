using Azure.AI.OpenAI;

namespace agenticai.app;

public static class LLMConfiguration
{
    public static AzureOpenAIClient GetAzureOpenAIClient()
    {
        var endpoint = ConfigurationUtils.GetConfigurationValue("AzureOpenAI:Endpoint");
        var apiKey = ConfigurationUtils.GetConfigurationValue("AzureOpenAI:ApiKey");

        if (string.IsNullOrWhiteSpace(endpoint))
        {
            throw new ArgumentException("Azure OpenAI endpoint cannot be null or empty.", nameof(endpoint));
        }

        if (string.IsNullOrWhiteSpace(apiKey))
        {
            throw new ArgumentException("Azure OpenAI API key cannot be null or empty.", nameof(apiKey));
        }

        var azureOpenAIClient = new AzureOpenAIClient(
            new Uri(endpoint),
             new System.ClientModel.ApiKeyCredential(apiKey));

        return azureOpenAIClient;
    }

    public static string GetDeploymentName()
    {
        var deploymentName = ConfigurationUtils.GetConfigurationValue("AzureOpenAI:DeploymentName");

        if (string.IsNullOrWhiteSpace(deploymentName))
        {
            throw new ArgumentException(
                "Azure OpenAI deployment name cannot be null or empty.",
                nameof(deploymentName));
        }

        return deploymentName;
    }
}