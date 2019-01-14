using System.Windows;
using AntRunner.Models;
using Colors = AntRunner.Interface.Colors;

namespace AntRunner.Controls.Ant.Views
{
    public partial class StatusControl
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(Colors), typeof(StatusControl), new UIPropertyMetadata(null));
        public static readonly DependencyProperty AntProperty = DependencyProperty.Register(nameof(Ant), typeof(AntWrapper), typeof(StatusControl), new UIPropertyMetadata(null));

        public Colors Color
        {
            get => (Colors)GetValue(ColorProperty);
            set => SetValue(ColorProperty, value);
        }

        public AntWrapper Ant
        {
            get => (AntWrapper)GetValue(AntProperty);
            set => SetValue(AntProperty, value);
        }

        public StatusControl()
        {
            InitializeComponent();
        }
    }
}
