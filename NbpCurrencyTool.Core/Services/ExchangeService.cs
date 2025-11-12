using NbpCurrencyTool.Core.Models;
using NbpCurrencyTool.Core.Interfaces;
using NbpCurrencyTool.Core.Utils;

namespace NbpCurrencyTool.Core.Services
{
    // Główna logika konwersji; SRP: tylko konwersje i dostęp do kursów
    public class ExchangeService
    {
        private readonly IExchangeRateProvider _provider;
        private readonly RatesNotifier _notifier;
        private List<ExchangeRate> _rates = new();

        public bool HasRates => _rates.Any();

        public ExchangeService(IExchangeRateProvider provider, RatesNotifier notifier)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        }

        // Aktualizuj kursy (wywoływane przez App / State)
        public async System.Threading.Tasks.Task UpdateRatesAsync()
        {
            var fetched = (await _provider.GetLatestRatesAsync()).ToList();
            if (!fetched.Any()) throw new InvalidOperationException("Pobrano pustą listę kursów.");
            _rates = fetched;
            _notifier.Notify(_rates);
        }

        public decimal Convert(string fromCode, string toCode, decimal amount)
        {
            if (string.IsNullOrWhiteSpace(fromCode) || string.IsNullOrWhiteSpace(toCode))
                throw new ArgumentException("Kody walut nie mogą być puste.");

            fromCode = fromCode.ToUpperInvariant();
            toCode = toCode.ToUpperInvariant();

            var from = _rates.FirstOrDefault(r => r.Code == fromCode);
            var to = _rates.FirstOrDefault(r => r.Code == toCode);

            if (from == null) throw new ArgumentException($"Brak kursu dla waluty źródłowej: {fromCode}");
            if (to == null) throw new ArgumentException($"Brak kursu dla waluty docelowej: {toCode}");

            // Kursy NBP: podawane są jako kurs średni dla N jednostek (przelicznik)
            // Aby szybko konwertować: najpierw na PLN, potem na docelową:
            // valueInPln = amount * (from.Rate / from.Unit)
            // result = valueInPln / (to.Rate / to.Unit)

            var valueInPln = amount * (from.Rate / from.Unit);
            var result = valueInPln / (to.Rate / to.Unit);
            return result;
        }

        public void PrintAvailableCurrencies()
        {
            var col = new CurrencyCollection(_rates);
            Console.WriteLine("Dostępne waluty:");
            foreach (var c in col)
            {
                Console.WriteLine($"{c.Code} - {c.Currency} (przelicznik: {c.Unit}, kurs: {c.Rate})");
            }
        }
    }
}