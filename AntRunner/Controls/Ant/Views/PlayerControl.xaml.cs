using System.Windows.Controls;
using System.Windows.Data;
using AntRunner.Controls.Ant.Converters;
using AntRunner.Converters;
using AntRunner.Models;

namespace AntRunner.Controls.Ant.Views
{
    public partial class PlayerControl
    {
        public PlayerControl(AntWrapper ant)
        {
            InitializeComponent();
            DataContext = ant;
            SetBinding(Canvas.TopProperty, new Binding("CurrentTile.Y") { Converter = new PositionToCanvasScaleConverter() });
            SetBinding(Canvas.LeftProperty, new Binding("CurrentTile.X") { Converter = new PositionToCanvasScaleConverter() });

            SetBinding(VisibilityProperty, new Binding("Health") { Converter = new HealthToVisibilityConverter() });
        }
    }
}
