using System;
using System.Globalization;
using System.Windows.Data;

namespace AntRunner.Controls.Ant.Converters
{
    public class LevelToBarWidthConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || (int) value == 0 ? 0 : (int) value * 1.5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
