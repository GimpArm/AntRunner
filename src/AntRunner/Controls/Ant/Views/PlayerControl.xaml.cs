using System;
using System.Windows;
using System.Windows.Data;
using AntRunner.Controls.Ant.Converters;
using AntRunner.Models;

namespace AntRunner.Controls.Ant.Views
{
    public partial class PlayerControl
    {
        public static readonly DependencyProperty XProperty = DependencyProperty.Register(nameof(X), typeof(double?), typeof(PlayerControl), new UIPropertyMetadata(null));
        public static readonly DependencyProperty YProperty = DependencyProperty.Register(nameof(Y), typeof(double?), typeof(PlayerControl), new UIPropertyMetadata(null));
        public static readonly DependencyProperty AnimationDurationProperty = DependencyProperty.Register(nameof(AnimationDuration), typeof(Duration), typeof(PlayerControl), new UIPropertyMetadata(null));

        public double? X
        {
            get => (double?)GetValue(XProperty);
            set => SetValue(XProperty, value);
        }

        public double? Y
        {
            get => (double?)GetValue(YProperty);
            set => SetValue(YProperty, value);
        }

        public Duration AnimationDuration
        {
            get => (Duration)GetValue(AnimationDurationProperty);
            set => SetValue(AnimationDurationProperty, value);
        }

        public void UpdateAnimationDuration()
        {
            AnimationDuration = new Duration(new TimeSpan(0, 0, 0, 0, (int)(GameManager.GameSpeed * 0.9)));
        }

        public PlayerControl(AntWrapper ant)
        {
            InitializeComponent();
            DataContext = ant;
            UpdateAnimationDuration();

            SetBinding(YProperty, new Binding("CurrentTile.Y") { NotifyOnTargetUpdated = true});
            SetBinding(XProperty, new Binding("CurrentTile.X") { NotifyOnTargetUpdated = true });

            SetBinding(VisibilityProperty, new Binding("Health") { Converter = new HealthToVisibilityConverter() });
        }
    }
}
