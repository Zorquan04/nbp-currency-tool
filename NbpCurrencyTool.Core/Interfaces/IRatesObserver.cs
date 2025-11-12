using NbpCurrencyTool.Core.Models;

namespace NbpCurrencyTool.Core.Interfaces
{
    public interface IRatesObserver
    {
        void OnRatesUpdated(IEnumerable<ExchangeRate> rates);
    }
}