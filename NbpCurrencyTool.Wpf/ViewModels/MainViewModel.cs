using NbpCurrencyTool.Core.Models;
using NbpCurrencyTool.Core.Services;
using NbpCurrencyTool.Wpf.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;

namespace NbpCurrencyTool.Wpf.ViewModels
{
    // ViewModel for the main window (MVVM)
    // Handles exchange calculation, currency list, UI visibility and status messages
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly ExchangeService _exchangeService;

        // List of available exchange rates
        public ObservableCollection<ExchangeRate> Rates { get; } = new();

        private ExchangeRate? _fromCurrency;
        public ExchangeRate? FromCurrency
        {
            get => _fromCurrency;
            set { _fromCurrency = value; OnPropertyChanged(); _convertCommand.RaiseCanExecuteChanged(); }
        }

        private ExchangeRate? _toCurrency;
        public ExchangeRate? ToCurrency
        {
            get => _toCurrency;
            set { _toCurrency = value; OnPropertyChanged(); _convertCommand.RaiseCanExecuteChanged(); }
        }

        private decimal _amount;
        public decimal Amount
        {
            get => _amount;
            set { _amount = value; OnPropertyChanged(); _convertCommand.RaiseCanExecuteChanged(); }
        }

        // Bound to TextBox: parses string input into decimal
        private string _amountText = "";
        public string AmountText
        {
            get => _amountText;
            set
            {
                _amountText = value;
                OnPropertyChanged();

                if (decimal.TryParse(_amountText, NumberStyles.Number, CultureInfo.CurrentCulture, out var val))
                    Amount = val;

                _convertCommand.RaiseCanExecuteChanged();
            }
        }

        private decimal _result;
        public decimal Result
        {
            get => _result;
            set { _result = value; OnPropertyChanged(); OnPropertyChanged(nameof(ResultDisplay)); }
        }

        private string _resultDisplay = "";
        public string ResultDisplay
        {
            get => _resultDisplay;
            private set { _resultDisplay = value; OnPropertyChanged(); }
        }

        private bool _showConverter = true;
        public bool ShowConverter
        {
            get => _showConverter;
            set => SetProperty(ref _showConverter, value);
        }

        private bool _showList = false;
        public bool ShowList
        {
            get => _showList;
            set => SetProperty(ref _showList, value);
        }

        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        private bool _showStatusMessage;
        public bool ShowStatusMessage
        {
            get => _showStatusMessage;
            set => SetProperty(ref _showStatusMessage, value);
        }

        private Brush _statusForeground = Brushes.Green;
        public Brush StatusForeground
        {
            get => _statusForeground;
            set { _statusForeground = value; OnPropertyChanged(); }
        }

        private readonly RelayCommand _convertCommand;
        public ICommand ConvertCommand => _convertCommand;

        public ICommand ShowConverterCommand { get; }
        public ICommand ShowListCommand { get; }
        public ICommand FetchCommand { get; }

        public MainViewModel(ExchangeService exchangeService)
        {
            _exchangeService = exchangeService;

            // Load initial rates
            _ = LoadRatesAsync();

            // Initialize commands
            _convertCommand = new RelayCommand(Convert, CanConvert);
            ShowConverterCommand = new RelayCommand(() => { ShowConverter = true; ShowList = false; }); // hide list
            ShowListCommand = new RelayCommand(() => { ShowConverter = false; ShowList = true; }); // hide converter
            FetchCommand = new RelayCommand(async () => 
            {
                try
                {
                    await LoadRatesAsync();
                    StatusForeground = Brushes.Green;
                    await ShowStatusAsync("Courses updated successfully!");
                }
                catch (Exception ex)
                {
                    StatusForeground = Brushes.Red;
                    await ShowStatusAsync(ex.Message);
                }
            });
        }

        // Load exchange rates from service
        private async Task LoadRatesAsync()
        {
            await _exchangeService.UpdateRatesAsync();

            Rates.Clear();
            foreach (var rate in _exchangeService.Rates.OrderBy(r => r.Code))
                Rates.Add(rate);

            FromCurrency = Rates.FirstOrDefault(r => r.Code == "PLN");
            ToCurrency = Rates.FirstOrDefault(r => r.Code == "USD");
        }

        // Determine if conversion can be executed
        private bool CanConvert() => FromCurrency != null && ToCurrency != null && Amount > 0;

        // Perform currency conversion
        public void Convert()
        {
            if (!CanConvert()) return;

            Result = _exchangeService.Convert(FromCurrency!.Code, ToCurrency!.Code, Amount);
            ResultDisplay = $"{Amount} {FromCurrency!.Code} = {Result:F3} {ToCurrency!.Code}";
        }

        // Show a temporary status message with automatic fade
        public async Task ShowStatusAsync(string message)
        {
            StatusMessage = message;
            ShowStatusMessage = true;
            await Task.Delay(2000);
            ShowStatusMessage = false;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        // Standard property changed notification
        private void OnPropertyChanged([CallerMemberName] string? propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Helper to simplify property setters
        private void SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (!Equals(field, value))
            {
                field = value;
                OnPropertyChanged(propertyName);
            }
        }
    }
}