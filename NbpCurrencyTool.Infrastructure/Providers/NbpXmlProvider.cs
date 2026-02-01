using System.Xml.Linq;
using NbpCurrencyTool.Core.Interfaces;
using NbpCurrencyTool.Core.Models;

namespace NbpCurrencyTool.Infrastructure.Providers
{
    public class NbpXmlProvider(string url) : IExchangeRateProvider
    {
        private readonly string _url = url ?? throw new ArgumentNullException(nameof(url));
        private static readonly HttpClient Http;

        static NbpXmlProvider()
        {
            Http = new HttpClient();
            Http.DefaultRequestHeaders.Add("User-Agent", "NbpCurrencyTool/1.0 (by KacperGumulak)");
            Http.DefaultRequestHeaders.Add("Accept", "application/xml");
            Http.DefaultRequestHeaders.Add("Accept-Language", "pl-PL");
        }
        
        public async Task<IEnumerable<ExchangeRate>> GetLatestRatesAsync()
        {
            string xml;
            try
            {
                var response = await Http.GetAsync(_url);
                response.EnsureSuccessStatusCode();
                xml = await response.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error downloading NBP data: " + ex.Message, ex);
            }

            try
            {
                var doc = XDocument.Parse(xml);
                // NBP structure: <exchange rate table> ... <item> ... <currency name>, <converter>, <currency_code>, <average exchange rate>
                var list = new List<ExchangeRate>();

                var ratesNodes = doc.Descendants("Rate"); // instead of "position"

                foreach (var r in ratesNodes)
                {
                    var name = (string?)r.Element("Currency") ?? "unknown";
                    var code = (string?)r.Element("Code") ?? "UNK";
                    var rateRaw = (string?)r.Element("Mid") ?? "0";

                    rateRaw = rateRaw.Replace(',', '.');

                    if (!decimal.TryParse(rateRaw, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var rate))
                    {
                        continue;
                    }

                    // All odds in table A have a conversion factor of 1
                    list.Add(new ExchangeRate(name, code.ToUpperInvariant(), rate, 1));
                }

                // Include PLN as a 1 for 1 unit rate - makes conversion easier
                list.Add(new ExchangeRate("polski złoty", "PLN", 1.0m, 1));

                return list;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("NBP data parsing error: " + ex.Message, ex);
            }
        }
    }
}