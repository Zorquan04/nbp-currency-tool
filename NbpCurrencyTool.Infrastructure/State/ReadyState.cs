using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class ReadyState : IAppState
    {
        private readonly AppStateContext _ctx;
        public ReadyState(AppStateContext ctx) => _ctx = ctx;

        public Task EnterAsync()
        {
            System.Console.WriteLine("[State] Ready - kursy dostępne.");
            return Task.CompletedTask;
        }
    }
}