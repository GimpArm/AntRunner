using System;
using System.Globalization;
using System.Windows.Data;
using AntRunner.Models;

namespace AntRunner.Controls.Tiles.Converters
{
    public class ShotLocationMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length != 3 || !(values[0] is int x) || !(values[1] is int distance) || !(values[2] is Direction d) || !(parameter is bool isTop)) return 0d;

            switch (d)
            {
                case Direction.Left:
                    x = !isTop ? x - distance : x;
                    break;
                case Direction.Up:
                    x = isTop ? x - distance : x;
                    break;
            }
            return x * 10d;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
