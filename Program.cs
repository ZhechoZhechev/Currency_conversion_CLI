﻿namespace Currency_conversion_CLI;

using Microsoft.VisualBasic;
using Newtonsoft.Json;
using RestSharp;
using System.Globalization;
using System.IO;

public class Program
{
    private static string apiKey = null!;
    private static HashSet<string> supportedCurrencies = new HashSet<string>();
    private static Dictionary<string, string> currencyRatesCache = new Dictionary<string, string>();

    private const string ENTER_AMOUNT_MESSAGE = "Enter a possitive number with not more than 2 digits after the decimal separator:";
    private const string ENTER_FROM_CURR_CODE_MESSAGE = "Enter a currency code according to ISO 4217 three letter currency code format to convert from";
    private const string ENTER_TO_CURR_CODE_MESSAGE = "Enter a currency code according to ISO 4217 three letter currency code format to convert to";
    private const string API_CLIENT_URL = "https://api.fastforex.io";

    /// <summary>
    /// Enttry point for the bissunes logic
    /// </summary>
    /// <param name="args"></param>
    public static void Main(string[] args)
    {
        LoadApiKey();
        LoadSupportedCurrencies();

        Console.WriteLine("Welcome to Currency Conversion CLI!");

        string dateInput = args.Length > 0 ? args[0] : string.Empty;

        while (!ValidateDateIput(dateInput))
        {
            Console.WriteLine("Please enter conversion rate date in the format YYYY-MM-DD:");
            dateInput = Console.ReadLine()!;
        }

        while (true)
        {
            var amount = GetValidAmout(ENTER_AMOUNT_MESSAGE);
            if (amount == -1) break;

            var fromCurrencyCode = ReturnCurrencyCodeIdExists(ENTER_FROM_CURR_CODE_MESSAGE);
            if (fromCurrencyCode == "END") break;

            var toCurrencyCode = ReturnCurrencyCodeIdExists(ENTER_TO_CURR_CODE_MESSAGE);
            if (toCurrencyCode == "END") break;

            if (currencyRatesCache.Count == 0)
            {
                CacheTheCurrencyRates(dateInput);
            }

            var currentRate = GetTheRate(fromCurrencyCode, toCurrencyCode);
            var result = Math.Round((amount * currentRate), 2);

            SaveConverionHistory(dateInput, fromCurrencyCode, toCurrencyCode, amount, result);

            Console.WriteLine($"{amount}{fromCurrencyCode} is {result}{toCurrencyCode}");

            Console.WriteLine("Do you want to perform another conversion? Type 'END' to exit or press Enter to continue.");
            if (Console.ReadLine().ToUpper() == "END") break;

        }
    }
    /// <summary>
    /// Loads the API key from the json file
    /// </summary>
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
    /// <summary>
    /// Validates if the entered date is in the correct format
    /// </summary>
    /// <param name="dateInput"></param>
    /// <returns></returns>
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
    /// <summary>
    /// Validates if the decimal has two or less digits after the separator
    /// </summary>
    /// <param name="amountInput"></param>
    /// <returns></returns>
    private static bool TwoDigitsAfterSeparator(string amountInput)
    {
        var parts = amountInput.Split(".");

        if (parts.Length == 1) return true;

        if (parts[1].Length > 2) return false;

        return true;

    }
    /// <summary>
    /// Gets list of supported currencies from the API and saves them in a dictionary
    /// </summary>
    /// <exception cref="Exception"></exception>
    private static void LoadSupportedCurrencies()
    {
        var client = new RestClient(API_CLIENT_URL);
        var request = new RestRequest("currencies")
            .AddParameter("api_key", apiKey);
        var response = client.Get(request);

        if (response.IsSuccessful)
        {
            var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content)["currencies"];
            var currenciesDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.ToString());

            foreach (var currency in currenciesDict)
            {
                supportedCurrencies.Add(currency.Key.ToUpper());
            }
        }
        else
        {
            throw new Exception("Failed to load supported currencies");
        }
        
    }
    /// <summary>
    /// Check if the entered currency code is valid
    /// </summary>
    /// <param name="promptMessage"></param>
    /// <returns></returns>
    private static string ReturnCurrencyCodeIdExists(string promptMessage)
    {
        while (true)
        {
            Console.WriteLine(promptMessage);
            var input = Console.ReadLine().ToUpper();

            if (input == "END") return input;
            if (supportedCurrencies.Contains(input)) return input;
        }
    }
    /// <summary>
    /// chaches all currenny rates for the given date
    /// </summary>
    /// <param name="date"></param>
    private static void CacheTheCurrencyRates(string date)
    {
        var client = new RestClient(API_CLIENT_URL);
        var request = new RestRequest("historical")
            .AddParameter("date", date)
            .AddParameter("api_key", apiKey);

        var response = client.Get(request);
        
        var data = JsonConvert.DeserializeObject<Dictionary<string, object>>(response.Content!)!["results"];
        currencyRatesCache = JsonConvert.DeserializeObject<Dictionary<string, string>>(data.ToString()!)!;
    }
    /// <summary>
    /// Converts the rate for the required currency
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    /// <returns></returns>
    private static decimal GetTheRate(string from, string to) 
    {
        var usdFromRate = decimal.Parse(currencyRatesCache[from]);
        var usdToRate = decimal.Parse(currencyRatesCache[to]);

        return usdToRate / usdFromRate;
    }
    /// <summary>
    /// Saves history of all conversion is a json file
    /// </summary>
    /// <param name="date"></param>
    /// <param name="fromCurrency"></param>
    /// <param name="toCurrency"></param>
    /// <param name="amount"></param>
    /// <param name="result"></param>
    private static void SaveConverionHistory(string date, string fromCurrency, string toCurrency, decimal amount, decimal result) 
    {
        var entry = new 
        {
            Date = date,
            Amount = amount,
            Base_currency = fromCurrency,
            Target_curency = toCurrency,
            Converted_amount = result
        };

        var history = new List<object>();
        if (File.Exists("conversions.json")) 
        {
            history = JsonConvert.DeserializeObject<List<object>>(File.ReadAllText("conversions.json"));

            if (history == null)
            {
                history = new List<object>();
            }
        }
        else
        {
            using FileStream fs = File.Create("conversions.json");
        }

        history.Add(entry);

        File.WriteAllText("conversions.json", JsonConvert.SerializeObject(history, Formatting.Indented));
    }
}


