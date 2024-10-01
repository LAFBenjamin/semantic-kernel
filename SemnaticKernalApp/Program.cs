using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
internal class Program
{
    private static void Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENIA_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("OPENIA_API_KEY environment variable is not set or empty.");
        }

        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-3.5-turbo", apiKey)
            .Build();

        Console.ReadKey();
    }
}