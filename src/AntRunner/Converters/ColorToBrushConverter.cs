using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Colors = AntRunner.Interface.Colors;

namespace AntRunner.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Colors color)) return Brushes.DimGray;
            switch (color)
            {
                case Colors.Red:
                    return Brushes.Red;
                case Colors.Blue:
                    return Brushes.DodgerBlue;
                case Colors.Green:
                    return Brushes.ForestGreen;
                case Colors.Orange:
                    return Brushes.Orange;
                case Colors.Pink:
                    return Brushes.Pink;
                case Colors.Yellow:
                    return Brushes.Yellow;
                case Colors.Gray:
                    return Brushes.Gray;
                case Colors.White:
                    return Brushes.White;
                default:
                   return Brushes.DimGray;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
