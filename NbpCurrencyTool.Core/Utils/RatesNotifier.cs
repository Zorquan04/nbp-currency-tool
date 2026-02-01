using NbpCurrencyTool.Core.Models;
using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Core.Utils
{
    // Simple notifier (Observable)
    public class RatesNotifier
    {
        private readonly List<IRatesObserver> _observers = new();

        public void Subscribe(IRatesObserver observer)
        {
            if (observer == null) throw new ArgumentNullException(nameof(observer));
            _observers.Add(observer);
        }

        public void Unsubscribe(IRatesObserver? observer)
        {
            if (observer == null) return;
            _observers.Remove(observer);
        }

        public void Notify(IEnumerable<ExchangeRate> rates)
        {
            var cachedRates = rates.ToList();
            
            foreach (var o in _observers)
            {
                try { o.OnRatesUpdated(cachedRates); } catch { /* ignoruj błędy obserwatorów */ }
            }
        }
    }
}