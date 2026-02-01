using NbpCurrencyTool.Core.Models;
using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.Observer
{
    // An example observer printing information to the console
    public class ConsoleRatesObserver : IRatesObserver
    {
        public void OnRatesUpdated(IEnumerable<ExchangeRate>? rates)
        {
            var count = rates?.Count() ?? 0;
            Console.WriteLine($"[Observer] Updated odds: {count} positions.");
        }
    }
}