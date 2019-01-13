using System;
using System.Globalization;
using System.Windows.Data;

namespace AntRunner.Controls.Tiles.Converters
{
    public class ShotDistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (((int?) value ?? 0) + 1) * 10;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
