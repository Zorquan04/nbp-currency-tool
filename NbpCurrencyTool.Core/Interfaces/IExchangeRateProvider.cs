using NbpCurrencyTool.Core.Models;

namespace NbpCurrencyTool.Core.Interfaces
{
    public interface IExchangeRateProvider
    {
        Task<IEnumerable<ExchangeRate>> GetLatestRatesAsync();
    }
}