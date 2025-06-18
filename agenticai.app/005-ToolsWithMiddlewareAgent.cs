using AutoGen.Core;
using AutoGen.OpenAI;
using AutoGen.OpenAI.Extension;

namespace AgenticAI.App;

public static class ToolsMiddlewareAgent
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

        var systemPrompt = @"
            You're an AI agent who can use tools to solve problems.
            You can use the following tools:
            1. UpperCase: Converts a string to uppercase.
            2. ConcatenateStrings: Concatenates an array of strings with a specified separator.
            3. CalculateTax: Calculates tax for a given amount based on the country.
            Use the tools by calling them with their names and required parameters.";
        var tools = new Tools();
        var toolsCallMiddleware = new FunctionCallMiddleware(
            functions:
            [
                tools.UpperCaseFunctionContract,
                tools.ConcatenateStringsFunctionContract,
                tools.CalculateTaxFunctionContract
            ],
            functionMap: new Dictionary<string, Func<string, Task<string>>>
            {
                { nameof(tools.UpperCase), tools.UpperCaseWrapper },
                { nameof(tools.ConcatenateStrings), tools.ConcatenateStringsWrapper },
                { nameof(tools.CalculateTax), tools.CalculateTaxWrapper }
            }
        );

        var agent = new OpenAIChatAgent(
            chatClient: client.GetChatClient(deploymentName),
            systemMessage: systemPrompt,
            name: "ToolsMiddlewareAgent",
            temperature: 1.0f,
            maxTokens: 2000
        )
            .RegisterMessageConnector()
            .RegisterMiddleware(toolsCallMiddleware)
            .RegisterPrintMessage();

        Console.WriteLine("Tools Middleware Agent is being Tested ...");

        Console.WriteLine("Testing #1 (UpperCase):");
        var response1 = await agent.SendAsync(
            "Can you convert this to uppercase? Hello, World!");
        Console.WriteLine("Response: " + response1.GetContent());
        Console.WriteLine("***********************************************************");

        Console.WriteLine("Testing #2 (ConcatenateStrings):");
        var response2 = await agent.SendAsync(
            "Can you concatenate these strings? ['Hello', 'World'] with a separator of ' - '");
        Console.WriteLine("Response: " + response2.GetContent());
        Console.WriteLine("***********************************************************");

        Console.WriteLine("Testing #3 (CalculateTax):");
        var response3 = await agent.SendAsync(
            "Can you calculate the tax for 1000 in Germany as well as 8000 in the USA?");
        Console.WriteLine("Response: " + response3.GetContent());
        Console.WriteLine("***********************************************************");

        Console.WriteLine("Testing #4 (Multilpe Tools Call):");
        var response4 = await agent.SendAsync(
            @"Can you convert 'hello' to uppercase and then concatenate 
                it with 'world' using a separator of ' - '
                and also calculate tax for 10000 in the USA?");

        Console.WriteLine("Response: " + response4.GetContent());
        Console.WriteLine("***********************************************************");
    }
}