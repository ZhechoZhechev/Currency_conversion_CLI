namespace Currency_conversion_CLI;

using Newtonsoft.Json;
using System.Globalization;
using System.IO;

public class Program
{
    private static string apiKey = null!;

    public static void Main(string[] args)
    {
        LoadApiKey();

        Console.WriteLine("Welcome to Currency Conversion CLI!");

        string dateInput = args.Length > 0 ? args[0] : string.Empty;

        while (!ValidateDateIput(dateInput))
        {
            Console.WriteLine("Please enter conversion rate date in the format YYYY-MM-DD:");
            dateInput = Console.ReadLine()!;
        }
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

    private static bool ValidateDateIput(string dateInput)
    {
        return DateTime.TryParseExact(dateInput, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);
    }
}


