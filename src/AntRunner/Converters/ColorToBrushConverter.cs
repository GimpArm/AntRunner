using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ItemColor = AntRunner.Interface.ItemColor;

namespace AntRunner.Converters
{
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ItemColor color)) return Brushes.DimGray;
            switch (color)
            {
                case ItemColor.Red:
                    return Brushes.Red;
                case ItemColor.Blue:
                    return Brushes.DodgerBlue;
                case ItemColor.Green:
                    return Brushes.ForestGreen;
                case ItemColor.Orange:
                    return Brushes.Orange;
                case ItemColor.Pink:
                    return Brushes.Pink;
                case ItemColor.Yellow:
                    return Brushes.Yellow;
                case ItemColor.Gray:
                    return Brushes.Gray;
                case ItemColor.White:
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
