using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class ReadyState : IAppState
    {
        public Task EnterAsync()
        {
            Console.WriteLine("[State] Ready - kursy dostępne.");
            return Task.CompletedTask;
        }
    }
}