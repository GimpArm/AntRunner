using System;
using System.Globalization;
using System.Windows.Data;
using AntRunner.Models;

namespace AntRunner.Converters
{
    public class DirectionToRotationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Direction direction)) return 0;

            switch (direction)
            {
                case Direction.Right:
                    return 90;
                case Direction.Down:
                    return 180;
                case Direction.Left:
                    return 270;
                case Direction.Up:
                    return 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is int degrees) || degrees == 0) return Direction.Up;

            if (degrees <= 90) return Direction.Right;
            if (degrees <= 180) return Direction.Down;
            if (degrees <= 270) return Direction.Left;
            return Direction.Up;
        }
    }
}
