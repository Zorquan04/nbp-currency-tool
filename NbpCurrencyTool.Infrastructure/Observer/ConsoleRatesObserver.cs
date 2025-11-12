using NbpCurrencyTool.Core.Models;
using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool
{
    // Przykładowy obserwator wypisujący info do konsoli
    public class ConsoleRatesObserver : IRatesObserver
    {
        public void OnRatesUpdated(System.Collections.Generic.IEnumerable<ExchangeRate> rates)
        {
            var count = rates?.Count() ?? 0;
            Console.WriteLine($"[Observer] Zaktualizowano kursy: {count} pozycji.");
        }
    }
}