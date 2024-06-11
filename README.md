It is a CLI currency convertor that accepts past date and uses the fastFORES API.

• The application accepts a command line argument for the date in format '2024-12-31'.
• The application is able to process multiple conversions.
• The application continuously validates all inputs until a correct one is submitted. Мonetary values are constrained to two decimal places. Currencies are in ISO 4217 three letter currency code format.
• The application is case-insensitive.
• The application is caching the exchange rates for each requested base currency. Subsequent conversions with this base currency are using the cached data, instead of calling the API.
• Each successful conversion is saved in a json file.
• The application is terminated by typing 'END' on any input.
• The application is loading the api_key for Fast Forex from a config.json file which must be ignored by the version control.
