using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class IdleState : IAppState
    {
        private readonly AppStateContext _ctx;
        public IdleState(AppStateContext ctx) => _ctx = ctx;
        public Task EnterAsync()
        {
            // nic szczególnego
            System.Console.WriteLine("[State] Idle");
            return Task.CompletedTask;
        }
    }
}