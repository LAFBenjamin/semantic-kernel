using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
public class Program
{
    public static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENIA_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
        {
            throw new InvalidOperationException("OPENIA_API_KEY environment variable is not set or empty.");
        }
       
        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-3.5-turbo", apiKey)
            .Build();

        kernel.Plugins.AddFromType<BookingPlugin>("booking");

        OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings(){
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };


        var ChatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        ChatHistory chatHistory = new ChatHistory();
        chatHistory.AddSystemMessage("Tu es un chatbot permettant de réserver des salles de réunion dans une entreprise.Tu dois récupérer comme info le jour, la date de début, la date de fin et le nombre de participants.");
        var chat = await ChatCompletion.GetChatMessageContentAsync(chatHistory);
        Console.WriteLine(chat);
        chatHistory.AddAssistantMessage(chat.ToString());

        while (true){
            chatHistory.AddUserMessage(Console.ReadLine()); 
            var reponse = await ChatCompletion.GetChatMessageContentAsync(chatHistory,settings,kernel);
            chatHistory.AddAssistantMessage(reponse.ToString());
            Console.WriteLine(reponse);
            Console.WriteLine(reponse.Metadata);
        }
        
        Console.ReadKey();
    }
}