using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AntRunner.Converters;
using AntRunner.Interface;
using AntRunner.Models;

namespace AntRunner.Controls.Tiles.Views
{
    public partial class HomeControl
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(ItemColor), typeof(HomeControl), new UIPropertyMetadata(null));

        public ItemColor Color
        {
            get => (ItemColor)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public HomeControl(MapTile tile)
        {
            InitializeComponent();
            DataContext = tile;
            SetBinding(Canvas.TopProperty, new Binding("Y") { Converter = new PositionToCanvasScaleConverter() });
            SetBinding(Canvas.LeftProperty, new Binding("X") { Converter = new PositionToCanvasScaleConverter() });
        }

        public HomeControl(ItemColor color, MapTile tile) : this(tile)
        {
            Color = color;
        }
    }
}
