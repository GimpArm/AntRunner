using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Input;
using System.Windows.Media;
using AntRunner.Controls.Ant.Views;
using AntRunner.Controls.Map.ViewModels;
using AntRunner.Events;
using AntRunner.Main.Views;
using AntRunner.Models;
using ItemColor = AntRunner.Interface.ItemColor;

namespace AntRunner.Main.ViewModels
{
    public class MainViewModel : NotifyBaseModel
    {
        private ICommand _runAgainCommand, _exitCommand, _toggleDebugCommand;

        private readonly MapTileControl _selectedMap;
        private readonly IList<AntWrapper> _players;
        private readonly MainWindow _control;

        private GameManager _gameManager;
        private MapViewModel _mapViewModel;

        private bool _gameStopped;
        public bool GameStopped
        {
            get => _gameStopped;
            set => SetValue(ref _gameStopped, value);
        }

        private bool _isDebug;
        public bool IsDebug
        {
            get => _isDebug;
            set
            {
                SetValue(ref _isDebug, value); 
                RaisePropertyChanged(nameof(WindowTitle));
            }
        }

        private ItemColor _winnerColor;
        public ItemColor WinnerColor
        {
            get => _winnerColor;
            set
            {
                SetValue(ref _winnerColor, value); 
                RaisePropertyChanged(nameof(WinnerLogo));
            }
        }

        private string _counterValue;
        public string CounterValue
        {
            get => _counterValue;
            set => SetValue(ref _counterValue, value);
        }

        public string WindowTitle => "Ant Runner" + (IsDebug ? " - Debug" : string.Empty);
        public ImageSource WinnerLogo => _players.FirstOrDefault(x => x.Color == WinnerColor)?.Icon;

        private string _winnerName;
        public string WinnerName
        {
            get => _winnerName;
            set => SetValue(ref _winnerName, value);
        }

        public MainViewModel(MainWindow control, MapTileControl map, IList<AntWrapper> players, bool isDebug = false)
        {
            _control = control;
            _selectedMap = map;
            IsDebug = isDebug;
            _players = players;
            _players.Shuffle();

            Initialize();
        }

        private void Initialize()
        {
            _control.MapArea.MapArea.Children.Clear();
            _gameManager = new GameManager(new Bitmap(_selectedMap.Map.UriSource.LocalPath), _players, IsDebug);
            _gameManager.OnGameOver += OnGameOver;
            _mapViewModel?.Dispose();
            _mapViewModel = new MapViewModel(_gameManager, _control.MapArea);
            LoadPlayers(_players);
            ScreenLockManager.DisableSleep();
        }

        public ICommand RunAgainCommand => _runAgainCommand ?? (_runAgainCommand = new DelegateCommand(RunAgain));
        public ICommand ExitCommand => _exitCommand ?? (_exitCommand = new DelegateCommand(_control.Close));
        public ICommand ToggleDebugCommand => _toggleDebugCommand ?? (_toggleDebugCommand = new DelegateCommand(ToggleDebug));

        private void LoadPlayers(IEnumerable<AntWrapper> players)
        {
            _control.PlayerArea.Children.Clear();
            foreach (var player in players)
            {
                _control.PlayerArea.Children.Add(new StatusControl {Color = player.Color, Ant = player});
                _mapViewModel.AddPlayer(player);
            }
        }

        private void OnGameOver(object sender, GameOverEventArgs e)
        {
            if (e.HasWinner)
            {
                WinnerColor = e.Color;
                WinnerName = e.Name;
            }

            StopGame();
        }

        public async void StartGame()
        {
            GameStopped = false;

            CounterValue = "3";
            await Task.Delay(1000);
            CounterValue = "2";
            await Task.Delay(1000);
            CounterValue = "1";
            await Task.Delay(1000);
            CounterValue = "Go!";
            await Task.Delay(1000);
            CounterValue = null;

            _gameManager.Start();
        }

        public void StopGame()
        {
            GameStopped = true;
            _gameManager.Stop();
            foreach (var p in _players)
            {
                _mapViewModel.ClearPlayer(p);
            }
            ScreenLockManager.EnableSleep();
        }

        private void RunAgain()
        {
            Initialize();
            StartGame();
        }

        private void ToggleDebug()
        {
            IsDebug = !IsDebug;
            _gameManager.IsDebug = IsDebug;
        }
    }
}
