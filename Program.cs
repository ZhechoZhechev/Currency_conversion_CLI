﻿namespace Currency_conversion_CLI;

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
            else
            {
                continue;
            }
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
}


