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
            Console.WriteLine("NBP Currency Tool - wersja studencka\n");

            // Tworzymy komponenty (zależności)
            var provider = new NbpXmlProvider("https://api.nbp.pl/api/exchangerates/tables/A?format=xml");
            var notifier = new RatesNotifier();
            var exchangeService = new ExchangeService(provider, notifier);
            var app = new AppStateContext(exchangeService);

            notifier.Subscribe(new ConsoleRatesObserver()); // demo observer (powiadamia w konsoli)

            // autoupdate na start
            await app.SetStateAsync(new FetchingState(app));

            // prosta pętla CLI
            while (true)
            {
                Console.WriteLine("\nKomendy: fetch | list | conv | exit");
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
                            Console.WriteLine("Brak kursów — użyj fetch.");
                        break;
                    case "conv":
                        if (!exchangeService.HasRates)
                        {
                            Console.WriteLine("Brak kursów — najpierw pobierz (fetch).");
                            break;
                        }
                        DoConversion(exchangeService);
                        break;
                    default:
                        Console.WriteLine("Nieznana komenda.");
                        break;
                }
            }

            Console.WriteLine("Program zakończył działanie!");
        }

        static void DoConversion(ExchangeService exchangeService)
        {
            Console.Write("Waluta źródłowa (kod, np. USD): ");
            var from = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
            Console.Write("Waluta docelowa (kod, np. EUR): ");
            var to = (Console.ReadLine() ?? "").Trim().ToUpperInvariant();
            Console.Write("Kwota (np. 123.45): ");
            var raw = Console.ReadLine() ?? "";

            raw = raw.Replace(',', '.'); // pozwala użytkownikowi wpisać zarówno przecinek jak i kropkę
            
            if (!decimal.TryParse(raw, System.Globalization.NumberStyles.Any,
                    System.Globalization.CultureInfo.InvariantCulture, out var amount))
            {
                Console.WriteLine("Niepoprawny format kwoty.");
                return;
            }

            try
            {
                var result = exchangeService.Convert(from, to, amount);
                Console.WriteLine($"{amount} {from} = {result:F4} {to}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Błąd: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Błąd operacji: {ex.Message}");
            }
        }
    }
}