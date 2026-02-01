using System.Windows;

namespace NbpCurrencyTool.Wpf
{
    public partial class MainWindow : Window
    {
        // Main window of the application
        // Initializes services, sets up the data context for MVVM
        public MainWindow()
        {
            InitializeComponent();

            var provider = new Infrastructure.Providers.NbpXmlProvider("https://api.nbp.pl/api/exchangerates/tables/A?format=xml");
            var notifier = new Core.Utils.RatesNotifier();
            var service = new Core.Services.ExchangeService(provider, notifier);

            DataContext = new ViewModels.MainViewModel(service);
        }
    }
}