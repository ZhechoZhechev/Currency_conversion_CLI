## About The Project
It is a CLI currency convertor that accepts past date and uses the fastFORES API.

<ul>
  <li>The application accepts a command line argument for the date in format '2024-12-31'</li>
  <li>The application is able to process multiple conversions</li>
  <li>The application continuously validates all inputs until a correct one is submitted. Мonetary values are constrained to two decimal places. Currencies are in ISO 4217 three letter currency code format</li>
  <li>The application is case-insensitive</li>
  <li>The application is caching the exchange rates for each requested base currency. Subsequent conversions with this base currency are using the cached data, instead of calling the API</li>
  <li>Each successful conversion is saved in a json file</li>
  <li>The application is terminated by typing 'END' on any input</li>
  <li>The application is loading the api_key for Fast Forex from a config.json file which must be ignored by the version control</li>
</ul>
<br /><br />

### Built With
<ul>
    <li><strong>C#</strong> - The programming language used to write the application.</li>
    <li><strong>.NET SDK</strong> - The software development kit used to build and publish the C# application.</li>
    <li><strong>Newtonsoft.Json</strong> - A popular JSON framework for .NET used for serializing and deserializing JSON data.</li>
    <li><strong>RestSharp</strong> - A library for making HTTP requests in .NET, used to interact with the Fast Forex API.</li>
    <li><strong>Fast Forex API</strong> - The external service providing historical exchange rate data.</li>
    <li><strong>Command Line Interface (CLI)</strong> - The interface through which users interact with the application by typing commands.</li>
    <li><strong>Git</strong> - Version control system used for managing and sharing the source code.</li>
    <li><strong>GitHub</strong> - Hosting service for version control using Git, where the project's repository is stored.</li>
</ul>
