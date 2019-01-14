using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Animation;
using AntRunner.Controls.Tiles.Converters;
using AntRunner.Converters;
using AntRunner.Models;

namespace AntRunner.Controls.Tiles.Views
{
    public partial class ShootControl
    {
        public static readonly DependencyProperty ShotDistanceProperty = DependencyProperty.Register(nameof(ShotDistance), typeof(int), typeof(ShootControl), new UIPropertyMetadata(10));
        public static readonly DependencyProperty DirectionProperty = DependencyProperty.Register(nameof(Direction), typeof(Direction), typeof(ShootControl), new UIPropertyMetadata(null));

        public int ShotDistance
        {
            get => (int)GetValue(ShotDistanceProperty);
            set => SetValue(ShotDistanceProperty, value);
        }

        public Direction Direction
        {
            get => (Direction)GetValue(DirectionProperty);
            set => SetValue(DirectionProperty, value);
        }


        public ShootControl(MapTile tile)
        {
            InitializeComponent();
            DataContext = tile;
            SetBinding(Canvas.TopProperty, new MultiBinding { ConverterParameter = true, Bindings = { new Binding("Y"), new Binding("ShotDistance") { ElementName = "ShotControlElement" }, new Binding("Direction"){ElementName = "ShotControlElement" } }, Converter = new ShotLocationMultiConverter() });
            SetBinding(Canvas.LeftProperty, new MultiBinding { ConverterParameter = false, Bindings = { new Binding("X"), new Binding("ShotDistance") { ElementName = "ShotControlElement" }, new Binding("Direction") { ElementName = "ShotControlElement" } }, Converter = new ShotLocationMultiConverter() });
            
            BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, new Duration(new TimeSpan(0, 0, 0, 0, 125))));
            
            var fadeOut = new DoubleAnimation(1, 0, new Duration(new TimeSpan(0, 0, 0, 0, 125)));
            fadeOut.Completed += Timeline_OnCompleted;
            BeginAnimation(OpacityProperty, fadeOut);
            
        }

        private void Timeline_OnCompleted(object sender, EventArgs e)
        {
            ((Canvas)Parent).Children.Remove(this);
        }
    }
}
