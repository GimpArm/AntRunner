using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AntRunner.Controls.Ant.Converters
{
    public class LevelToBarWidthConverter : IMultiValueConverter
    {
        public object Convert(object[] value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return (double)0;

            if (!(value[1] is double))
                return (double)0;

            double maxLevel = (double)value[1];
            if (maxLevel == 0)
                return (double)0;

            int currentLevel = (int)value[0];
            return maxLevel * ((double)currentLevel / 100.0);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
