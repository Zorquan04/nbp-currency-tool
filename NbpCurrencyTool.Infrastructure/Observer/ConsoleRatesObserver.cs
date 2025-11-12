using NbpCurrencyTool.Core.Models;
using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.Observer
{
    // Przykładowy obserwator wypisujący info do konsoli
    public class ConsoleRatesObserver : IRatesObserver
    {
        public void OnRatesUpdated(IEnumerable<ExchangeRate>? rates)
        {
            var count = rates?.Count() ?? 0;
            Console.WriteLine($"[Observer] Zaktualizowano kursy: {count} pozycji.");
        }
    }
}