using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class IdleState : IAppState
    {
        public Task EnterAsync()
        {
            // nothing special
            Console.WriteLine("[State] Inactivity");
            return Task.CompletedTask;
        }
    }
}