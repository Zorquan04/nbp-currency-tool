using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace NbpCurrencyTool.Wpf.Utils
{
    // converts true/false to Visible/Collapsed
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (Visibility)value == Visibility.Visible;
    }
}