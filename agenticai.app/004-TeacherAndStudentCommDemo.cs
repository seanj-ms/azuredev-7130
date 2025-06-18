using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;
using Microsoft.SemanticKernel.Agents;

namespace AgenticAI.App;

public static class TeacherStudentCommDemo
{
    public static async Task RunAsync()
    {
        var deploymentName = LLMConfiguration.GetDeploymentName();

        if (string.IsNullOrWhiteSpace(deploymentName))
        {
            throw new InvalidOperationException("Deployment name is not configured.");
        }

        var client = LLMConfiguration.GetAzureOpenAIClient();

        if (client == null)
        {
            throw new InvalidOperationException("Azure OpenAI client is not initialized.");
        }

        var teacherSystemPrompt = @"
            You're a teacher AI agent who creates a mid-school level random math problem for a student AI agent to solve.
            And checks answers from the student AI agent.
            If the answer is correct, you will say 'Correct!' and you stop the conversation by saying COMPLETE.
            if the answer is incorrect, you will say 'Incorrect! Try again.' and you will continue the conversation by asking the student to fix it.
            After 3 incorrect attempts, you will say 'Too many attempts, and you stop the conversation by saying COMPLETE.
        ";

        var teacherAgent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            systemMessage: teacherSystemPrompt,
            name: "TeacherAgent",
            temperature: 1.0f,
            maxTokens: 2000
        )
            .RegisterMessageConnector()
            .RegisterMiddleware(
                async (messages, options, agent, cancellationToken) =>
                {
                    var reply = await agent.GenerateReplyAsync(messages, options, cancellationToken);

                    if (reply.GetContent()?.ToLower().Contains("complete") == true)
                    {
                        Console.WriteLine("Teacher Agent: " + reply.GetContent());

                        return new TextMessage(
                            Role.Assistant,
                            GroupChatExtension.TERMINATE,
                            from: reply.From
                        );
                    }

                    return reply;
                }
            )
            .RegisterPrintMessage();

        var studentSystemPrompt = @"
            You are an outstanding student AI agent who solves math problems given by a teacher AI agent.
            If the teacher says your answer is correct, you will say 'Thank you, teacher!' and you stop the conversation by saying COMPLETE.
            If the teacher says your answer is incorrect, you will say 'I will try again.' 
            and you will continue the conversation by trying to fix it.
        ";

        var studentAgent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            systemMessage: studentSystemPrompt,
            name: "StudentAgent",
            temperature: 1.0f,
            maxTokens: 2000
        )
            .RegisterMessageConnector()
            .RegisterPrintMessage();

        var conversation = await studentAgent.InitiateChatAsync(
            receiver: teacherAgent,
            message: "Hello, teacher! Please give me a math problem to solve.",
            maxRound: 10
        );

        foreach (var message in conversation)
        {
            Console.WriteLine($"{message.From}: {message.GetContent()}");
        }

        Console.WriteLine("Conversation completed.");
        Console.WriteLine("Application Processing Completed.");
    }
}