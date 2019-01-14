using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;
using AntRunner.Interface;
using AntRunner.Models;

namespace AntRunner.Main.Converters
{
    public class PlayerLookupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Dictionary<Colors, AntWrapper> lookup)) return null;
            if (!(parameter is string colorString)) return null;

            if (!Enum.TryParse<Colors>(colorString, true, out var color)) return null;
            return lookup[color];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
