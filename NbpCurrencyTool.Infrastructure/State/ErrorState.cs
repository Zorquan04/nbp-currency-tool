using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class ErrorState : IAppState
    {
        private readonly AppStateContext _ctx;
        private readonly Exception _ex;
        public ErrorState(AppStateContext ctx, Exception ex) { _ctx = ctx; _ex = ex; }

        public Task EnterAsync()
        {
            Console.WriteLine("[State] Error: " + _ex.Message);
            // Możesz tu zapisać log lub ustawić retry policy
            return Task.CompletedTask;
        }
    }
}