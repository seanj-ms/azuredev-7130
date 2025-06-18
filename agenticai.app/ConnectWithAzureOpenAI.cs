using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;

namespace agenticai.app;

public class ConnectWithAzureOpenAI
{
    public async Task<string> RunAsync(string input = "")
    {
        var client = LLMConfiguration.GetAzureOpenAIClient();
        var deploymentName = LLMConfiguration.GetDeploymentName();

        if (client == null)
        {
            throw new InvalidOperationException("Azure OpenAI client is not initialized.");
        }

        if (string.IsNullOrWhiteSpace(deploymentName))
        {
            throw new InvalidOperationException("Deployment name is not configured.");
        }

        var systemPrompt = @"
            You are an AI assistant agent designed to assist users with various tasks using Azure OpenAI services and AutoGen.
            Please ensure that you follow the guidelines and best practices for interacting with users.
            Your answer should be concise, informative, and relevant to the user's request.
        ";

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "AzureOpenAIAgent",
            systemMessage: systemPrompt,
            temperature: 0.7f,
            maxTokens: 2000
        )
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        if (string.IsNullOrWhiteSpace(input))
        {
            input = "Can you generate a C# code that returns and displays 100th fibonacci number?";
        }

        var userMessage = new TextMessage(
            role: Role.User,
            content: input);

        var response = await agent.SendAsync(userMessage);

        if (response is TextMessage textResponse)
        {
            Console.WriteLine($"Response: {textResponse.GetContent()}");
        }
        else
        {
            Console.WriteLine("Unexpected response type received.");
        }

        Console.WriteLine("End of the Application Processing!");

        return response?.GetContent()!;
    }
}