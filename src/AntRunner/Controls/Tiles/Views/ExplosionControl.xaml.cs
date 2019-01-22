using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using AntRunner.Converters;
using AntRunner.Models;

namespace AntRunner.Controls.Tiles.Views
{
    public partial class ExplosionControl
    {
        public ExplosionControl(MapTile tile)
        {
            InitializeComponent();
            DataContext = tile;
            SetBinding(Canvas.TopProperty, new Binding("Y") { Converter = PositionToCanvasScaleConverter.Default });
            SetBinding(Canvas.LeftProperty, new Binding("X") { Converter = PositionToCanvasScaleConverter.Default });

            BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 250))));

            var fadeOut = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 250)));
            fadeOut.Completed += Timeline_OnCompleted;
            BeginAnimation(OpacityProperty, fadeOut);
        }

        private void Timeline_OnCompleted(object sender, EventArgs e)
        {
            ((Canvas)Parent).Children.Remove(this);
        }
    }
}
