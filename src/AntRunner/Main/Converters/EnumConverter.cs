using System;
using System.Windows.Data;

namespace AntRunner.Main.Converters
{
    public class EnumConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var returnValue = 0;
            if (parameter is Type enumType)
            {
                returnValue = (int)Enum.Parse(enumType, value?.ToString() ?? "0");
            }
            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var enumValue = default(Enum);
            if (parameter is Type enumType)
            {
                enumValue = (Enum)Enum.Parse(enumType, value?.ToString() ?? "0");
            }
            return enumValue;
        }
    }
}