using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using OpenAI.Chat;

namespace AgenticAI.App;

public static class RAGMiddlewareInAgent
{
    public static async Task RunAsync()
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
            You're a helpful AI assistant that provides information and answers to user queries using the context provided.
            You use the context to answer questions accurately and provide relevant information.
        ";

        var totalTokenCount = 0;

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "CustomMiddlewareAgent",
            systemMessage: systemPrompt,
            temperature: 0.7f,
            maxTokens: 2000
        )
            .RegisterMessageConnector()
            .RegisterMiddleware(
                async (messages, option, agent, cancellationToken) =>
                {
                    var today = DateTime.UtcNow;
                    var todayMessage = new TextMessage(
                        role: Role.System,
                        content: $"Today's date is {today:yyyy-MM-dd}. Please use this context to answer the user's question."
                    );

                    messages = messages.Concat([todayMessage]);

                    return await agent.GenerateReplyAsync(messages, option, cancellationToken);
                })
            .RegisterPrintMessage();

        var reply = await agent.SendAsync(
            new TextMessage(
                role: Role.User,
                content: "What's today's date?"));

        if (reply is TextMessage textReply)
        {
            Console.WriteLine($"Response: {textReply.GetContent()}");
        }
        else
        {
            Console.WriteLine("Unexpected response type received.");
        }

        Console.WriteLine("End of the Application Processing!");
    }
}