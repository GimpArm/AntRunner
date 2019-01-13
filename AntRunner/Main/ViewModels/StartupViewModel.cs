using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Waf.Applications;
using System.Windows.Input;
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

        public StartupViewModel(StartupWindow control, bool isDebug = false)
        {
            _control = control;
            IsDebug = isDebug;
            LoadMaps();
            Players.CollectionChanged += PlayersOnCollectionChanged;
        }

        private void PlayersOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(CanStart));
        }

        private void LoadMaps()
        {
            var first = true;
            foreach (var map in Directory.GetFiles("Maps\\", "*.bmp"))
            {
                var tile = new MapTileControl(map);
                tile.InputBindings.Add(new MouseBinding(LoadMapCommand, new MouseGesture(MouseAction.LeftClick)) { CommandParameter = tile });
                if (first)
                {
                    tile.Selected = true;
                    _selectedMap = tile;
                    first = false;
                }
                _control.MapSelectionArea.Children.Add(tile);
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
