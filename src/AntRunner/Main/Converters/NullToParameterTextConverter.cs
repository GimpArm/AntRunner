using System;
using System.Globalization;
using System.Windows.Data;
using AntRunner.Models;

namespace AntRunner.Main.Converters
{
    public class NullToParameterTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is AntWrapper ant)) return parameter;
            return ant.Name ?? parameter;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
