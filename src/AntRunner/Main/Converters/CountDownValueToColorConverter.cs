using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace AntRunner.Main.Converters
{
    public class CountDownValueToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is string sValue)) return Brushes.Red;

            switch (sValue)
            {
                case "2":
                case "1":
                    return Brushes.Yellow;
                case "Go!":
                    return Brushes.Green;
                default:
                    return Brushes.Red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
