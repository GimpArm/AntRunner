using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Waf.Applications;
using System.Windows.Input;
using System.Windows.Media;
using AntRunner.Controls.Ant.Views;
using AntRunner.Controls.Map.ViewModels;
using AntRunner.Events;
using AntRunner.Game.Interface.Models;
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

        #region Member - IsDebug
        private bool _isDebug;
        public bool IsDebug
        {
            get => _isDebug;
            set
            {
                SetValue(ref _isDebug, value);
                this.RaisePropertyChanged(nameof(WindowTitle));
            }
        }
        #endregion

        #region Member - WinnerColor
        private ItemColor _winnerColor;
        public ItemColor WinnerColor
        {
            get => _winnerColor;
            set
            {
                SetValue(ref _winnerColor, value);
            }
        }
        #endregion

        #region Member - CounterValue
        private string _counterValue;
        public string CounterValue
        {
            get => _counterValue;
            set => SetValue(ref _counterValue, value);
        }
        #endregion

        #region Member - IsModePlaying
        public bool IsModePlaying
        {
            get => _gameManager?.CurrentRunningMode == GameRunningModeType.Playing;
            set
            {
                if (value)
                {
                    _gameManager.CurrentRunningMode = GameRunningModeType.Playing;
                }
            }
        }
        #endregion

        #region Member - IsModePause
        public bool IsModePause
        {
            get => _gameManager?.CurrentRunningMode == GameRunningModeType.Pause;
            set
            {
                if (value)
                {
                    _gameManager.CurrentRunningMode = GameRunningModeType.Pause;
                }
            }
        }
        #endregion

        #region Member - IsModeNextStep
        public bool IsModeNextStep
        {
            get => _gameManager?.CurrentRunningMode == GameRunningModeType.NextStep;
            set
            {
                if (value)
                {
                    _gameManager.CurrentRunningMode = GameRunningModeType.NextStep;
                }
            }
        }
        #endregion

        #region Member - MenuItems
        public ObservableCollection<MenuItemViewModel> MenuItems { get; set; } = new ObservableCollection<MenuItemViewModel>();
        #endregion

        #region Member - IsTurboMode
        public bool IsTurboMode
        {
            get => GameManager.GameSpeed == 25;
            set
            {
                GameManager.GameSpeed = value ? 25 : 250;
                _mapViewModel?.UpdateGameSpeedOnPlayer();
                RaisePropertyChanged("IsTurboMode");
            }
        }
        #endregion

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
            if (_selectedMap.Map != null)
            {
                _gameManager = new GameManager(_players, IsDebug, new Bitmap(_selectedMap.Map.UriSource.LocalPath));
            }
            else
            {
                _gameManager = new GameManager(_players, IsDebug);
            }
            _gameManager.OnGameOver += OnGameOver;
            _gameManager.OnRunningModeChanged += OnRunningModeChanged;

            RefreshComponents();

            _mapViewModel?.Dispose();
            _mapViewModel = new MapViewModel(_gameManager, _control.MapArea);
            LoadPlayers(_players);
            ScreenLockManager.DisableSleep();
        }

        private void RefreshComponents()
        {
            var externalComponents = new MenuItemViewModel
            {
                DisplayText = "External Components"
            };
            MenuItems.Add(externalComponents);

            foreach (var currentComponent in _gameManager.ExternalComponents)
            {
                if (!currentComponent.IsAutoRun)
                {
                    externalComponents.ChildItems.Add(new MenuItemViewModel
                    {
                        DisplayText = currentComponent.DisplayText,
                        ExecuteAction = () =>
                        {
                            currentComponent.Start();
                        }
                    });
                }
            }
        }

        private void OnRunningModeChanged(object sender, EventArgs e)
        {
            var runningMode = (sender as GameRunningModeType?);
            if (runningMode.HasValue)
            {
                RaisePropertyChanged("IsModePlaying");
                RaisePropertyChanged("IsModePause");
                RaisePropertyChanged("IsModeNextStep");
            }
        }

        public ICommand RunAgainCommand => _runAgainCommand ?? (_runAgainCommand = new DelegateCommand(RunAgain));
        public ICommand ExitCommand => _exitCommand ?? (_exitCommand = new DelegateCommand(_control.Close));
        public ICommand ToggleDebugCommand => _toggleDebugCommand ?? (_toggleDebugCommand = new DelegateCommand(ToggleDebug));

        private void LoadPlayers(IEnumerable<AntWrapper> players)
        {
            _control.PlayerArea.Children.Clear();
            foreach (var player in players)
            {
                _control.PlayerArea.Children.Add(new StatusControl { Color = player.Color, Ant = player });
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
