using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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

        public MapTileControl(FileSystemInfo info)
        {
            InitializeComponent();

            NameLabel.Content = Path.GetFileNameWithoutExtension(info.Name).Replace('_', ' ');
            Map = new BitmapImage(new Uri(info.FullName));
            Image.Source = Map;
        }

        public MapTileControl()
        {
            InitializeComponent();
            NameLabel.Content = "Generate Map";
            this.Image.Source = BitmapFrame.Create(new Uri("pack://application:,,,/AntRunner;component/Images/RandomMap.png", UriKind.RelativeOrAbsolute));
        }

        public MapTileControl(string filename) : this(new FileInfo(filename))
        {

        }
    }
}
