using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using System.Windows.Input;
using AntRunner.Interface;
using AntRunner.Main.Views;
using AntRunner.Models;

namespace AntRunner.Main.ViewModels
{
    public class StartupViewModel : NotifyBaseModel
    {
        private ICommand _startGameCommand, _loadMapCommand;

        private readonly StartupWindow _control;
        private MapTileControl _selectedMap;

        public ObservableCollection<AntWrapper> Players { get; } = new ObservableCollection<AntWrapper>();
        public bool CanStart => _selectedMap != null && Players.Any();

        private bool _isDebug;
        public bool IsDebug
        {
            get => _isDebug;
            set => SetValue(ref _isDebug, value);
        }

        public StartupViewModel(StartupWindow control, FileSystemInfo map, IEnumerable<Ant> ants, bool isDebug = false)
        {
            _control = control;
            IsDebug = isDebug;
            LoadMaps(map);
            Players.CollectionChanged += PlayersOnCollectionChanged;
            LoadAnts(ants);
        }

        private void PlayersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(CanStart));
        }

        private void LoadMaps(FileSystemInfo selected)
        {
            var first = true;
            foreach (var map in Directory.GetFiles("Maps\\", "*.bmp"))
            {
                var info = new FileInfo(map);
                var tile = new MapTileControl(info);
                tile.InputBindings.Add(new MouseBinding(LoadMapCommand, new MouseGesture(MouseAction.LeftClick)) { CommandParameter = tile });
                if (first)
                {
                    tile.Selected = true;
                    _selectedMap = tile;
                    first = false;
                }
                else if (selected != null && info.FullName.Equals(selected.FullName))
                {
                    LoadMap(tile);
                    selected = null;
                }
                _control.MapSelectionArea.Children.Add(tile);
            }

            if (selected != null)
            {
                var tile = new MapTileControl(selected);
                tile.InputBindings.Add(new MouseBinding(LoadMapCommand, new MouseGesture(MouseAction.LeftClick)) { CommandParameter = tile });
                _control.MapSelectionArea.Children.Add(tile);
                LoadMap(tile);
            }
        }

        private void LoadAnts(IEnumerable<Ant> ants)
        {
            if (ants == null) return;
            foreach (var ant in ants)
            {
                var slot = _control.LoadAntArea.Children.OfType<LoadAntControl>().Reverse().FirstOrDefault(x => x.Ant == null);
                if (slot == null) return;
                var color = Enum.GetValues(typeof(ItemColor)).OfType<ItemColor>().FirstOrDefault(c => c != ItemColor.None && Players.All(p => p.Color != c));
                slot.Ant = new AntWrapper(ant, color);
                Players.Add(slot.Ant);
            }
        }

        public ICommand StartGameCommand => _startGameCommand ?? (_startGameCommand = new DelegateCommand(StartGame));
        public ICommand LoadMapCommand => _loadMapCommand ?? (_loadMapCommand = new DelegateCommand(LoadMap));

        private void StartGame()
        {
            var gameWindow = new MainWindow(_selectedMap, Players, IsDebug) {Owner = _control};
            _control.Hide();
            gameWindow.ShowDialog();
            _control.Show();
        }

        private void LoadMap(object input)
        {
            if (!(input is MapTileControl map)) return;
            if (_selectedMap != null)
            {
                _selectedMap.Selected = false;
            }
            map.Selected = true;
            _selectedMap = map;
            RaisePropertyChanged(nameof(CanStart));
        }
    }
}
