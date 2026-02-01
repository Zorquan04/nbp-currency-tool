using NbpCurrencyTool.Core.Services;
using NbpCurrencyTool.Core.Utils;
using NbpCurrencyTool.Infrastructure.Observer;
using NbpCurrencyTool.Infrastructure.State;
using NbpCurrencyTool.Infrastructure.Providers;

namespace NbpCurrencyTool.App
{
    static class Program
    {
        private static async Task Main()
        {
            Console.WriteLine("NBP Currency Tool - student version\n");

            // We create components (dependencies)
            var provider = new NbpXmlProvider("https://api.nbp.pl/api/exchangerates/tables/A?format=xml");
            var notifier = new RatesNotifier();
            var exchangeService = new ExchangeService(provider, notifier);
            var app = new AppStateContext(exchangeService);

            notifier.Subscribe(new ConsoleRatesObserver()); // demo observer (notifies in console)

            // autoupdate on start
            await app.SetStateAsync(new FetchingState(app));

            // simple CLI loop
            while (true)
            {
                Console.WriteLine("\nCommands: fetch | list | conv | exit");
                Console.Write("> ");
                var cmd = Console.ReadLine()?.Trim().ToLowerInvariant();

                if (string.IsNullOrEmpty(cmd)) continue;
                if (cmd == "exit") break;

                switch (cmd)
                {
                    case "fetch":
                        await app.SetStateAsync(new FetchingState(app));
                        break;
                    case "list":
                        if (exchangeService.HasRates)
                            exchangeService.PrintAvailableCurrencies();
                        else
                            Console.WriteLine("No courses - use fetch.");
                        break;
                    case "conv":
                        if (!exchangeService.HasRates)
                        {
                            Console.WriteLine("No courses - fetch first.");
                            break;
                        }
                        DoConversion(exchangeService);
                        break;
                    default:
                        Console.WriteLine("Unknown command.");
                        break;
                }
            }

            Console.WriteLine("The program has finished. See you soon!");
        }

        static void DoConversion(ExchangeService exchangeService)
        {
            Console.Write("Source Currency (code, e.g. USD): ");
            var from = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
            Console.Write("Target currency (code, e.g. EUR): ");
            var to = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
            Console.Write("Amount (e.g. 123.45): ");
            var raw = Console.ReadLine() ?? "";

            raw = raw.Replace(',', '.'); // allows the user to enter both a comma and a period

            if (!decimal.TryParse(raw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var amount))
            {
                Console.WriteLine("Incorrect amount format.");
                return;
            }

            try
            {
                var result = exchangeService.Convert(from, to, amount);
                Console.WriteLine($"{amount} {from} = {result:F4} {to}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Operation error: {ex.Message}");
            }
        }
    }
}