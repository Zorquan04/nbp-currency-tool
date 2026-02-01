using System.Collections;
using NbpCurrencyTool.Core.Models;

namespace NbpCurrencyTool.Core.Utils
{
    // Iterator: allows you to iterate through the available currencies
    public class CurrencyCollection(IEnumerable<ExchangeRate>? rates) : IEnumerable<ExchangeRate>
    {
        private readonly List<ExchangeRate> _rates = rates?.ToList() ?? new List<ExchangeRate>();

        public IEnumerator<ExchangeRate> GetEnumerator() => _rates.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}