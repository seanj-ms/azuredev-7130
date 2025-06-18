using System;
using System.Threading.Tasks;
using AgenticAI.App;

class Program
{
    static async Task Main(string[] args)
    {
        int choice;
        do
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Connect with Azure OpenAI");
            Console.WriteLine("2. Custom Middleware in Agent");
            Console.WriteLine("3. RAG Middleware in Agent");
            Console.WriteLine("4. Teacher-Student Communication Demo");
            Console.WriteLine("5. Tools Middleware Agent");
            
            Console.WriteLine("0. Exit");

            Console.Write("Enter your choice: ");

            var input = Console.ReadLine();

            if (!int.TryParse(input, out choice))
            {
                Console.WriteLine("Invalid input. Please enter a number.");
                continue;
            }

            switch (choice)
            {
                case 1:
                    var connectWithAzureOpenAI = new ConnectWithAzureOpenAI();
                    await connectWithAzureOpenAI.RunAsync();
                    break;
                case 0:
                    Console.WriteLine("Exiting...");
                    break;
                case 2:
                    await CustomMiddlewareInAgent.RunAsync();
                    break;
                case 3:
                    await RAGMiddlewareInAgent.RunAsync();
                    break;
                case 4:
                    await TeacherStudentCommDemo.RunAsync();
                    break;
                case 5:
                    await ToolsMiddlewareAgent.RunAsync();
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        } while (choice != 0);
    }
}