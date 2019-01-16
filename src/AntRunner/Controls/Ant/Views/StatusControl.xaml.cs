using System.Windows;
using AntRunner.Models;
using ItemColor = AntRunner.Interface.ItemColor;

namespace AntRunner.Controls.Ant.Views
{
    public partial class StatusControl
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(Color), typeof(ItemColor), typeof(StatusControl), new UIPropertyMetadata(null));
        public static readonly DependencyProperty AntProperty = DependencyProperty.Register(nameof(Ant), typeof(AntWrapper), typeof(StatusControl), new UIPropertyMetadata(null));

        public ItemColor Color
        {
            get => (ItemColor)GetValue(ColorProperty);
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
