using NbpCurrencyTool.Core.Models;
using NbpCurrencyTool.Core.Services;
using NbpCurrencyTool.Core.Interfaces;
using NbpCurrencyTool.Core.Utils;

namespace NbpCurrencyTool.Tests
{
    [TestFixture]
    public class ExchangeServiceTests
    {
        private ExchangeService _service;
        private MockProvider _mockProvider;
        private RatesNotifier _notifier;

        [SetUp]
        public void Setup()
        {
            _mockProvider = new MockProvider();
            _notifier = new RatesNotifier();
            _service = new ExchangeService(_mockProvider, _notifier);
        }

        [Test]
        public async Task UpdateRatesAsync_SetsRatesCorrectly()
        {
            // Before UpdateRatesAsync the list is empty
            Assert.That(_service.HasRates, Is.False);

            await _service.UpdateRatesAsync();

            // It should be full after downloading
            Assert.That(_service.HasRates, Is.True);
            Assert.That(_mockProvider.LatestRates.Count, Is.EqualTo(3));
        }

        [Test]
        public async Task Convert_ReturnsCorrectValue()
        {
            await _service.UpdateRatesAsync();

            decimal amount = 100m;

            // rates: USD 4, EUR 2
            decimal result = _service.Convert("USD", "EUR", amount);

            // USD->PLN: 100 * 4/1 = 400
            // PLN->EUR: 400 / 2 = 200
            Assert.That(result, Is.EqualTo(200m));
        }

        [Test]
        public async Task Convert_Throws_OnUnknownCurrency()
        {
            await _service.UpdateRatesAsync();

            Assert.Throws<ArgumentException>(() => _service.Convert("XXX", "EUR", 10));
            Assert.Throws<ArgumentException>(() => _service.Convert("USD", "YYY", 10));
        }

        [Test]
        public void Convert_Throws_OnEmptyCodes()
        {
            Assert.Throws<ArgumentException>(() => _service.Convert("", "EUR", 10));
            Assert.Throws<ArgumentException>(() => _service.Convert("USD", "", 10));
        }

        // Mock provider for testing
        private class MockProvider : IExchangeRateProvider
        {
            public List<ExchangeRate> LatestRates { get; } =
            [
                new ExchangeRate("US Dollar", "USD", 4m, 1),
                new ExchangeRate("Euro", "EUR", 2m, 1),
                new ExchangeRate("Polish Zloty", "PLN", 1m, 1)
            ];

            public Task<IEnumerable<ExchangeRate>> GetLatestRatesAsync()
            {
                return Task.FromResult<IEnumerable<ExchangeRate>>(LatestRates);
            }
        }
    }
}