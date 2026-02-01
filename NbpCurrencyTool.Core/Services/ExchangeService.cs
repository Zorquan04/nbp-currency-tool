using NbpCurrencyTool.Core.Models;
using NbpCurrencyTool.Core.Interfaces;
using NbpCurrencyTool.Core.Utils;

namespace NbpCurrencyTool.Core.Services
{
    // Main conversion logic; SRP: conversions and rate access only
    public class ExchangeService(IExchangeRateProvider provider, RatesNotifier notifier)
    {
        private readonly IExchangeRateProvider _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        private readonly RatesNotifier _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        private List<ExchangeRate> _rates = new();

        public IReadOnlyList<ExchangeRate> Rates => _rates;
        public bool HasRates => _rates.Any();

        // Update Rates (triggered by App/State)
        public async Task UpdateRatesAsync()
        {
            var fetched = (await _provider.GetLatestRatesAsync()).ToList();
            if (!fetched.Any()) throw new InvalidOperationException("Empty course list downloaded.");
            _rates = fetched;
            _notifier.Notify(_rates);
        }

        public decimal Convert(string fromCode, string toCode, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(fromCode) || string.IsNullOrWhiteSpace(toCode))
                throw new ArgumentException("Currency codes cannot be empty.");

            fromCode = fromCode.ToUpperInvariant();
            toCode = toCode.ToUpperInvariant();

            var from = _rates.FirstOrDefault(r => r.Code == fromCode);
            var to = _rates.FirstOrDefault(r => r.Code == toCode);

            if (from == null) throw new ArgumentException($"No rate for source currency: {fromCode}");
            if (to == null) throw new ArgumentException($"No rate for target currency: {toCode}");

            // NBP rates: are provided as an average rate for N units (converter)
            // To quickly convert: first to PLN, then to the target currency:
            // valueInPln = amount * (from.Rate / from.Unit)
            // result = valueInPln / (to.Rate / to.Unit)

            var valueInPln = amount * (from.Rate / from.Unit);
            var result = valueInPln / (to.Rate / to.Unit);
            return result;
        }

        public void PrintAvailableCurrencies()
        {
            var col = new CurrencyCollection(_rates);
            Console.WriteLine("Available currencies:");
            foreach (var c in col)
            {
                Console.WriteLine($"{c.Code} - {c.Currency} (converter: {c.Unit}, course: {c.Rate})");
            }
        }
    }
}