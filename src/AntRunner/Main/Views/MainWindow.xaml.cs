using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using AntRunner.Main.ViewModels;
using AntRunner.Models;

namespace AntRunner.Main.Views
{
    public partial class MainWindow
    {
        public MainWindow(MapTileControl map, IList<AntWrapper> players, bool isDebug = false)
        {
            InitializeComponent();
            DataContext = new MainViewModel(this, map, players, isDebug);
            ContentRendered += OnContentRendered;
            Closing += OnClosing;
            KeyUp += OnKeyUp;
        }

        //Window.InputBindings is not working! Use an event instead
        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.F5) return;
            if (((MainViewModel)DataContext).ToggleDebugCommand.CanExecute(null))
            {
                ((MainViewModel)DataContext).ToggleDebugCommand.Execute(null);
            }
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (((MainViewModel) DataContext).GameStopped) return;
            if (MessageBox.Show("Game in progress, are you sure?", "Game Running", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK)
            {
                ((MainViewModel)DataContext).StopGame();
                return;
            }

            e.Cancel = true;
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            ContentRendered -= OnContentRendered;
            ((MainViewModel)DataContext).StartGame();
        }
    }
}
