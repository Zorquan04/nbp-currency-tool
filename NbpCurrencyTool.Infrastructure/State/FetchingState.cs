using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class FetchingState(AppStateContext ctx) : IAppState
    {
        public async Task EnterAsync()
        {
            Console.WriteLine("[State] Fetching latest rates...");
            try
            {
                await ctx.ExchangeService.UpdateRatesAsync();
                Console.WriteLine("Pobrano kursy pomyślnie.");
                // Po udanym pobraniu ustawiamy stan Idle/Ready
                await ctx.SetStateAsync(new ReadyState());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd pobierania: {ex.Message}");
                await ctx.SetStateAsync(new ErrorState(ex));
            }
        }
    }
}