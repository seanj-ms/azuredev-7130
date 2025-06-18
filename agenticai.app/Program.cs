using System;
using System.Threading.Tasks;
using agenticai.app;

class Program
{
    static async Task Main(string[] args)
    {
        int choice;
        do
        {
            Console.WriteLine("Menu:");
            Console.WriteLine("1. Connect with Azure OpenAI");
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
                    var azureOpenAI = new ConnectWithAzureOpenAI();
                    await azureOpenAI.RunAsync();
                    break;
                case 0:
                    Console.WriteLine("Exiting...");
                    break;
                default:
                    Console.WriteLine("Invalid option. Try again.");
                    break;
            }
        } while (choice != 0);
    }
}