using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class FetchingState : IAppState
    {
        private readonly AppStateContext _ctx;
        public FetchingState(AppStateContext ctx) => _ctx = ctx;

        public async Task EnterAsync()
        {
            Console.WriteLine("[State] Fetching latest rates...");
            try
            {
                await _ctx.ExchangeService.UpdateRatesAsync();
                Console.WriteLine("Pobrano kursy pomyślnie.");
                // Po udanym pobraniu ustawiamy stan Idle/Ready
                await _ctx.SetStateAsync(new ReadyState(_ctx));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd pobierania: {ex.Message}");
                await _ctx.SetStateAsync(new ErrorState(_ctx, ex));
            }
        }
    }
}