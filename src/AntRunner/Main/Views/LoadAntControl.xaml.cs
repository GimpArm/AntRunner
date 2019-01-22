using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Waf.Applications;
using System.Windows;
using System.Windows.Input;
using AntRunner.Interface;
using AntRunner.Models;
using Microsoft.Win32;

namespace AntRunner.Main.Views
{
    public partial class LoadAntControl 
    {
        public static readonly DependencyProperty AntProperty = DependencyProperty.Register(nameof(Ant), typeof(AntWrapper), typeof(LoadAntControl), new UIPropertyMetadata(null));
        public static readonly DependencyProperty PlayersProperty = DependencyProperty.Register(nameof(Players), typeof(ObservableCollection<AntWrapper>), typeof(LoadAntControl), new UIPropertyMetadata(null));

        private ICommand _loadAntCommand, _selectColorCommand, _unloadAntCommand;

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
        public ICommand UnloadAntCommand => _unloadAntCommand ?? (_unloadAntCommand = new DelegateCommand(UnloadAnt));

        private void LoadAnt()
        {
            var dialog = new OpenFileDialog
            {
                Filter = "Assembly files (*.dll)|*.dll",
                RestoreDirectory = true
            };
            if (dialog.ShowDialog() == false) return;

            try
            {
                var ant = AntLoader.Load(dialog.FileName, out var domain);
                if (ant == null)
                {
                    Ant = null;
                    return;
                }

                ItemColor color;
                if (Ant != null)
                {
                    color = Ant.Color;
                    Players.Remove(Ant);
                    Ant.Dispose();
                }
                else
                {
                    color = Enum.GetValues(typeof(ItemColor)).OfType<ItemColor>().FirstOrDefault(c => c != ItemColor.None && Players.All(p => p.Color != c));
                }
                Ant = new AntWrapper(ant, domain, color);
                Players.Add(Ant);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SelectColor(object target)
        {
            if (Ant == null)
            {
                LoadAnt();
            }
            if (Ant == null || !(target is FrameworkElement element) || !(element.Tag is ItemColor color)) return;
            if (Players.Any(p => p.Color == color)) return;
            Ant.SetColor(color);
        }

        private void UnloadAnt()
        {
            if (Ant == null) return;
            var current = Ant;
            Ant = null;
            Players.Remove(current);
            current.Dispose();
        }
    }
}
