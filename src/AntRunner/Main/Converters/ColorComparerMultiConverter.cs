using System;
using System.Globalization;
using System.Windows.Data;
using AntRunner.Interface;

namespace AntRunner.Main.Converters
{
    public class ColorComparerMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is ItemColor boxColor) || !(values[1] is ItemColor antColor)) return false;
            return boxColor == antColor;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
