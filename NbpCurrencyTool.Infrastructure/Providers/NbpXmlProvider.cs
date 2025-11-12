using System.Xml.Linq;
using NbpCurrencyTool.Core.Interfaces;
using NbpCurrencyTool.Core.Models;

namespace NbpCurrencyTool.Providers
{
    public class NbpXmlProvider : IExchangeRateProvider
    {
        private readonly string _url;
        private static readonly HttpClient _http = new HttpClient();

        public NbpXmlProvider(string url)
        {
            _url = url ?? throw new ArgumentNullException(nameof(url));
        }

        public async Task<IEnumerable<ExchangeRate>> GetLatestRatesAsync()
        {
            string xml;
            try
            {
                xml = await _http.GetStringAsync(_url);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Błąd pobierania danych NBP: " + ex.Message, ex);
            }

            try
            {
                var doc = XDocument.Parse(xml);
                // Struktura NBP: <tabela_kursow> ... <pozycja> ... <nazwa_waluty>, <przelicznik>, <kod_waluty>, <kurs_sredni>
                var list = new List<ExchangeRate>();

                var pozycje = doc.Descendants("pozycja");
                foreach (var p in pozycje)
                {
                    var name = (string)p.Element("nazwa_waluty")! ?? "";
                    var code = (string)p.Element("kod_waluty")! ?? "";
                    var unitRaw = (string)p.Element("przelicznik")! ?? "1";
                    var rateRaw = (string)p.Element("kurs_sredni")! ?? "0";

                    if (!int.TryParse(unitRaw, out var unit)) unit = 1;
                    
                    rateRaw = rateRaw.Replace(',', '.');

                    if (!decimal.TryParse(rateRaw, System.Globalization.NumberStyles.Any,
                        System.Globalization.CultureInfo.InvariantCulture, out var rate))
                    {
                        continue;
                    }

                    list.Add(new ExchangeRate(name, code.ToUpperInvariant(), rate, unit));
                }

                // Dołącz PLN jako kurs 1 dla 1 jednostki — ułatwia konwersję
                list.Add(new ExchangeRate("polski złoty", "PLN", 1.0m, 1));

                return list;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Błąd parsowania danych NBP: " + ex.Message, ex);
            }
        }
    }
}