using System.Text;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
public class Program
{
    public static async Task Main(string[] args)
    {
        var apiKey = Environment.GetEnvironmentVariable("OPENIA_API_KEY");
        if (string.IsNullOrEmpty(apiKey))
            throw new InvalidOperationException("OPENIA_API_KEY environment variable is not set or empty.");
        
       
        var kernel = Kernel.CreateBuilder()
            .AddOpenAIChatCompletion("gpt-3.5-turbo",apiKey: apiKey)
            .Build();

        kernel.Plugins.AddFromType<BookingPlugin>("booking");
        kernel.Plugins.AddFromType<DatePlugin>("Date");
         
        OpenAIPromptExecutionSettings settings = new OpenAIPromptExecutionSettings(){
            FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
        };


        var ChatCompletion = kernel.GetRequiredService<IChatCompletionService>();
        ChatHistory chatHistory = new ChatHistory();

        StringBuilder prompt = new StringBuilder();
        prompt.AppendLine("Tu es un assistant permettant de réserver des salles de réunion dans une entreprise. ");
        prompt.AppendLine("Tu dois récupérer les informations suivantes : la date de la réunion, l'heure de début, l'heure de fin, le nombre de participants, et la localisation de la réunion. ");
        prompt.AppendLine("La seule information obligatoire est le nombre de participants. ");
         prompt.AppendLine("Si l'utilisateur ne precise pas d'horaire tu fais la recherche seulement sur la date de la réunion et tu laisses start time et end time à une valeur vide. ");
        prompt.AppendLine("Une salle peut être réservée dans la plage des horaires disponibles. ");
        prompt.AppendLine("tu dois indiquer que l'heure n'est pas valide si l'utilisateur demande une réservation pour une journée passée ou pour aujourd'hui avec une heure inférieure à l'heure actuelle. ");
        prompt.AppendLine("Si l'utilisateur demande une réservation pour aujourd'hui et ne précise pas d'heure, l'heure de début doit être l'heure actuelle. ");
        prompt.AppendLine("Lorsque tu proposes une liste de salles de réunion, tu dois préciser les créneaux horaires disponibles pour chaque salle ainsi que la date de la réunion. ");
        prompt.AppendLine("Si aucune salle n'est disponible, tu dois indiquer qu'aucune salle n'a été trouvée et demander si l'utilisateur souhaite effectuer une recherche pour une autre date. ");
        prompt.AppendLine("Avant la confirmation de la réservation, tu dois récapituler les informations de la réunion (jour et heure) ainsi que les détails de la salle et demander une confirmation de la réservation. ");
        prompt.AppendLine("À chaque changement de paramètre, tu dois vérifier l'information avec la fonction get_booking_room_available. ");
        prompt.AppendLine("À chaque réponse de la fonction get_booking_room_available, tu dois prendre en compte uniquement ce résultat pour afficher les disponibilités des salles. ");
        prompt.AppendLine("tu doiste baser sur le champ RoomAvailabilities avec les créneaux disponibles pour définir les créneaux horaires. ");
        prompt.AppendLine("Le format de la date doit être DD/MM/YYYY et celui de l'heure HH:MM. ");
        prompt.AppendLine("Une réservation est possible uniquement dans le futur, jamais dans le passé. ");
        prompt.AppendLine("tu ne dois jamais afficher des informations de réservation inconnues.");

        chatHistory.AddSystemMessage(prompt.ToString());
            
         
        var  welcome = "Bonjour, comment puis-je t'aider aujourd'hui ?";
        chatHistory.AddAssistantMessage(welcome);
        
        Console.WriteLine(welcome);
        
        while (true){
            chatHistory.AddUserMessage(Console.ReadLine()); 
            var reponse = await ChatCompletion.GetChatMessageContentAsync(chatHistory,settings,kernel);
            chatHistory.AddAssistantMessage(reponse.ToString());
            Console.WriteLine(reponse);
        }
    }
}