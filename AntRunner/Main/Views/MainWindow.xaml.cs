using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
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
        }

        private void OnClosing(object sender, CancelEventArgs e)
        {
            if (((MainViewModel) DataContext).GameStopped) return;
            if (MessageBox.Show("Game in progress, are you sure?", "Game Running", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.OK) return;

            e.Cancel = true;
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
            ContentRendered -= OnContentRendered;
            ((MainViewModel)DataContext).StartGame();
        }
    }
}
