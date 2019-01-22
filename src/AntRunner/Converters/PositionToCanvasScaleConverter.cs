using System;
using System.Globalization;
using System.Windows.Data;

namespace AntRunner.Converters
{
    public class PositionToCanvasScaleConverter : IValueConverter
    {
        public static readonly PositionToCanvasScaleConverter Default = new PositionToCanvasScaleConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int?) value * 10 ?? 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int?)value / 10 ?? 0;
        }
    }
}
