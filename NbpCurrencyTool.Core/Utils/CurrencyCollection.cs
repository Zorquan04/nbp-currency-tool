using System.Collections;
using NbpCurrencyTool.Core.Models;

namespace NbpCurrencyTool.Core.Utils
{
    // Iterator: umożliwia iterowanie po dostępnych walutach
    public class CurrencyCollection(IEnumerable<ExchangeRate>? rates) : IEnumerable<ExchangeRate>
    {
        private readonly List<ExchangeRate> _rates = rates?.ToList() ?? new List<ExchangeRate>();

        public IEnumerator<ExchangeRate> GetEnumerator() => _rates.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}