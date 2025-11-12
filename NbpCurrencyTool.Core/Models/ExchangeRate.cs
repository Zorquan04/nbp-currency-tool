namespace NbpCurrencyTool.Core.Models
{
    public record ExchangeRate(string Currency, string Code, decimal Rate, int Unit);
}