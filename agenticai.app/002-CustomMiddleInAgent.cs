using AutoGen.Core;
using AutoGen.OpenAI;
using OpenAI.Chat;

namespace AgenticAI.App;

public static class CustomMiddlewareInAgent
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
            You're a helpful AI assistant that provides information and answers to user queries.
            You can always tell jokes and engage casual conversations.
        ";

        var totalTokenCount = 0;

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            name: "CustomMiddlewareAgent",
            systemMessage: systemPrompt,
            temperature: 0.7f,
            maxTokens: 2000
        )
            .RegisterMiddleware(
                async (messages, option, agent, cancellationToken) =>
                {
                    var reply = await agent.GenerateReplyAsync(messages, option, cancellationToken);

                    if (reply is MessageEnvelope<ChatCompletion> chatCompletionEnvelope)
                    {
                        var tokenCount = chatCompletionEnvelope.Content.Usage?.TotalTokenCount ?? 0;

                        totalTokenCount += tokenCount;

                        Console.WriteLine($"Total tokens used so far: {totalTokenCount}");
                    }

                    return reply;
                })
            .RegisterMiddleware(new OpenAIChatRequestMessageConnector());

        var reply = await agent.SendAsync(
            new TextMessage(
                role: Role.User,
                content: "Tell me a joke about Batman Joker"));

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