namespace Currency_conversion_CLI;

using Newtonsoft.Json;
using System.Globalization;
using System.IO;

public class Program
{
    private static string apiKey = null!;

    private const string AMOUNT_MESSAGE = "Enter a possitive number with not more than 2 digits after the decimal separator:";

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

        while (true)
        {
            var amount = GetValidAmout(AMOUNT_MESSAGE);
            if (amount == -1) break;
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

    private static decimal GetValidAmout(string promptMessage)
    {
        while (true)
        {
            Console.WriteLine(promptMessage);

            var amountInput = Console.ReadLine()!;
            if (amountInput.ToUpper() == "END") return -1;

            if (decimal.TryParse(amountInput, out decimal amount) && amount > 0
                && TwoDigitsAfterSeparator(amountInput)) return amount;

            Console.WriteLine("Invalid amount!");
        }

    }
    private static bool TwoDigitsAfterSeparator(string amountInput)
    {
        var parts = amountInput.Split(".");

        if (parts.Length == 1) return true;

        if (parts[1].Length > 2) return false;

        return true;

    }
}


