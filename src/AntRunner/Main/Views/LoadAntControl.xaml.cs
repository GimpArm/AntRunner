using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AntRunner.Interface;
using AntRunner.Models;
using Microsoft.Win32;

namespace AntRunner.Main.Views
{
    public partial class LoadAntControl 
    {
        public static readonly DependencyProperty AntProperty = DependencyProperty.Register(nameof(Ant), typeof(AntWrapper), typeof(LoadAntControl), new UIPropertyMetadata(null));
        public static readonly DependencyProperty PlayersProperty = DependencyProperty.Register(nameof(Players), typeof(ObservableCollection<AntWrapper>), typeof(LoadAntControl), new UIPropertyMetadata(null));

        private ICommand _loadAntCommand, _selectColorCommand;

        public AntWrapper Ant
        {
            get => (AntWrapper)GetValue(AntProperty);
            set => SetValue(AntProperty, value);
        }

        public ObservableCollection<AntWrapper> Players
        {
            get => (ObservableCollection<AntWrapper>)GetValue(PlayersProperty);
            set => SetValue(AntProperty, value);
        }

        public LoadAntControl()
        {
            InitializeComponent();
        }

        public ICommand LoadAntCommand => _loadAntCommand ?? (_loadAntCommand = new DelegateCommand(LoadAnt));
        public ICommand SelectColorCommand => _selectColorCommand ?? (_selectColorCommand = new DelegateCommand(SelectColor));

        private void LoadAnt()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Assembly files (*.dll)|*.dll",
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == false) return;

            var loadedAssembly = Assembly.LoadFrom(dialog.FileName);

            var antType = loadedAssembly.GetTypes().FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Ant)));
            if (antType == null)
            {
                MessageBox.Show("Could not find class derived from AntRunner.Interface.Ant");
                return;
            }

            var ant = Activator.CreateInstance(antType) as Ant;
            if (ant == null)
            {
                MessageBox.Show($"Could not initialize Ant class {antType.Name}");
                return;
            }

            Colors color;
            if (Ant != null)
            {
                Players.Remove(Ant);
                color = Ant.Color;
            }
            else
            {
                color = Enum.GetValues(typeof(Colors)).OfType<Colors>().FirstOrDefault(c => c != Colors.None && Players.All(p => p.Color != c));
            }
            Ant = new AntWrapper(ant, color);
            Players.Add(Ant);
        }

        private void SelectColor(object target)
        {
            if (Ant == null)
            {
                LoadAnt();
            }
            if (Ant == null || !(target is FrameworkElement element) || !(element.Tag is Colors color)) return;
            if (Players.Any(p => p.Color == color)) return;
            Ant.SetColor(color);
        }
    }
}
