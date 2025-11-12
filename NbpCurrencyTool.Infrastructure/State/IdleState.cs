using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class IdleState : IAppState
    {
        public Task EnterAsync()
        {
            // nic szczególnego
            Console.WriteLine("[State] Bezczynność");
            return Task.CompletedTask;
        }
    }
}