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


        public PlayerControl(AntWrapper ant)
        {
            InitializeComponent();
            DataContext = ant;

            SetBinding(YProperty, new Binding("CurrentTile.Y") { NotifyOnTargetUpdated = true});
            SetBinding(XProperty, new Binding("CurrentTile.X") { NotifyOnTargetUpdated = true });

            SetBinding(VisibilityProperty, new Binding("Health") { Converter = new HealthToVisibilityConverter() });
        }
    }
}
