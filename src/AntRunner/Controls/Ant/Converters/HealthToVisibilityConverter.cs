using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AntRunner.Controls.Ant.Converters
{
    public class HealthToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (string.Equals(parameter as string, "invert", StringComparison.OrdinalIgnoreCase))
                return value == null || (int)value == 0 ? Visibility.Visible : Visibility.Collapsed;
            return value == null || (int)value == 0 ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
