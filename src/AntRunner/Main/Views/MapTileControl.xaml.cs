using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AntRunner.Enums;

namespace AntRunner.Main.Views
{
    public partial class MapTileControl
    {
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(nameof(Selected), typeof(bool), typeof(MapTileControl), new UIPropertyMetadata(false));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(MapTileControl), new UIPropertyMetadata(null));

        public bool Selected
        {
            get => (bool)GetValue(SelectedProperty);
            set => SetValue(SelectedProperty, value);
        }

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public BitmapImage Map { get; }

        public MapSize MapSize { get; set; } = MapSize.Small;

        public string FullName { get; }

        public MapTileControl()
        {
            InitializeComponent();
            NameLabel.Text = "Generate Map";
            Image.Source = BitmapFrame.Create(new Uri("pack://application:,,,/AntRunner;component/Images/RandomMap.png", UriKind.RelativeOrAbsolute));
            MapSizeText.Visibility = Visibility.Collapsed;
            MapSizeCombo.Visibility = Visibility.Visible;
        }

        public MapTileControl(string filename)
        {
            InitializeComponent();
            var info = new FileInfo(filename);
            NameLabel.Text = Path.GetFileNameWithoutExtension(info.Name).Replace('_', ' ');
            FullName = info.FullName;
            Map = new BitmapImage(new Uri(info.FullName));
            Image.Source = Map;
            MapSizeText.Visibility = Visibility.Visible;
            MapSizeCombo.Visibility = Visibility.Collapsed;
            MapSizeText.Text = $"{Map.PixelWidth} x {Map.PixelHeight}";
        }
    }
}
