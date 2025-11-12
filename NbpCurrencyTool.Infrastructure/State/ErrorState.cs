using NbpCurrencyTool.Core.Interfaces;

namespace NbpCurrencyTool.Infrastructure.State
{
    public class ErrorState(Exception ex) : IAppState
    {
        public Task EnterAsync()
        {
            Console.WriteLine("[State] Błąd: " + ex.Message);
            return Task.CompletedTask;
        }
    }

}