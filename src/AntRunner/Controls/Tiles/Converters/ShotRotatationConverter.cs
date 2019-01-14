using System;
using System.Globalization;
using System.Windows.Data;

namespace AntRunner.Controls.Ant.Converters
{
    public class ShotRotatationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool?)value ?? false) ? 90 : 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
