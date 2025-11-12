using NbpCurrencyTool.Core.Interfaces;
using NbpCurrencyTool.Core.Services;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class AppStateContext(ExchangeService exchangeService)
    {
        private IAppState _state = new IdleState();
        public ExchangeService ExchangeService { get; } = exchangeService ?? throw new ArgumentNullException(nameof(exchangeService));

        public async Task SetStateAsync(IAppState newState)
        {
            _state = newState ?? throw new ArgumentNullException(nameof(newState));
            await _state.EnterAsync();
        }
    }
}