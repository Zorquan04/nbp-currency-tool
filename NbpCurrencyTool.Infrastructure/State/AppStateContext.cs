using NbpCurrencyTool.Core.Interfaces;
using NbpCurrencyTool.Core.Services;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class AppStateContext
    {
        private IAppState _state;
        public ExchangeService ExchangeService { get; }

        public AppStateContext(ExchangeService exchangeService)
        {
            ExchangeService = exchangeService ?? throw new ArgumentNullException(nameof(exchangeService));
            _state = new IdleState(this);
        }

        public async Task SetStateAsync(IAppState newState)
        {
            _state = newState ?? throw new ArgumentNullException(nameof(newState));
            await _state.EnterAsync();
        }
    }
}