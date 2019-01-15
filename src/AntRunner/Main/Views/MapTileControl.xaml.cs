using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

namespace AntRunner.Main.Views
{
    public partial class MapTileControl
    {
        public static readonly DependencyProperty SelectedProperty = DependencyProperty.Register(nameof(Selected), typeof(bool), typeof(MapTileControl), new UIPropertyMetadata(false));

        public bool Selected
        {
            get => (bool)GetValue(SelectedProperty);
            set => SetValue(SelectedProperty, value);
        }

        public BitmapImage Map { get; }

        public MapTileControl(FileSystemInfo info)
        {
            InitializeComponent();
            
            NameLabel.Content = Path.GetFileNameWithoutExtension(info.Name);
            Map = new BitmapImage(new Uri(info.FullName));
            Image.Source = Map;
        }

        public MapTileControl(string filename) : this(new FileInfo(filename))
        {
            
        }
    }
}
