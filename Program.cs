namespace Currency_conversion_CLI;

using Newtonsoft.Json;
using System.IO;

public class Program
{
    private static string apiKey = null!;

    public static void Main(string[] args)
    {
        LoadApiKey();
    }
    private static void LoadApiKey()
    {
        try
        {
            var file = File.ReadAllText("config.json");
            var config = JsonConvert.DeserializeObject<Dictionary<string, string>>(file);
            apiKey = config!["api_key"];
        }
        catch (Exception)
        {
            Console.WriteLine("Unable to load the API key.");
        }

    }
}


