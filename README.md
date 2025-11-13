# NBP Currency Tool

A virtual currency exchange office in C# (Console App) that downloads current exchange rates from the National Bank of Poland website and allows conversions between any currency.

## Features
- Downloading exchange rates from the National Bank of Poland (Table A)
- Conversion between selected currencies
- Displaying available currencies in the console
- Handling errors and missing rates
- Expandable with additional observers (RatesNotifier)

## Technologies
- .NET 9
- NUnit 3 – unit tests
- HttpClient + LINQ to XML for data retrieval and parsing

## UML Diagram
![NbpCurrencyToolUML](assets/nbp_currency_tool_uml.png)

## Launch
1. Clone the repository:
```bash
git clone https://github.com/Zorquan04/nbp-currency-tool.git
```
2. Open the project

3. Make sure the startup project is `App`

4. Run the application -> F5 or `dotnet run`

## Tests
The unit tests use NUnit3 and include:
- Downloading exchange rates (mock)
- Currency conversion logic
- Support Incorrect data

Run using `dotnet test`

## Usage example:

Commands available in the console:
- fetch : get the latest exchange rates
- list : display available currencies
- conv : convert the amount between currencies
- exit : exit the program