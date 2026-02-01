using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class FetchingState(AppStateContext ctx) : IAppState
    {
        public async Task EnterAsync()
        {
            Console.WriteLine("[State] Downloading latest rates...");
            try
            {
                await ctx.ExchangeService.UpdateRatesAsync();
                Console.WriteLine("Rates downloaded successfully.");
                // After successful download, we set the Idle/Ready state
                await ctx.SetStateAsync(new ReadyState());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Download error: {ex.Message}");
                await ctx.SetStateAsync(new ErrorState(ex));
            }
        }
    }
}