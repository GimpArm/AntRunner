using System.Windows.Controls;
using System.Windows.Data;
using AntRunner.Converters;
using AntRunner.Models;

namespace AntRunner.Controls.Tiles.Views
{
    public partial class BombControl
    {
        public BombControl(MapTile tile)
        {
            InitializeComponent();
            DataContext = tile;
            SetBinding(Canvas.TopProperty, new Binding("Y") { Converter = PositionToCanvasScaleConverter.Default });
            SetBinding(Canvas.LeftProperty, new Binding("X") { Converter = PositionToCanvasScaleConverter.Default });
        }
    }
}
