using System.Collections.Generic;
using System.Drawing;
using System.Waf.Applications;
using System.Windows.Input;
using AntRunner.Controls.Ant.Views;
using AntRunner.Controls.Map.ViewModels;
using AntRunner.Events;
using AntRunner.Interface;
using AntRunner.Main.Views;
using AntRunner.Models;

namespace AntRunner.Main.ViewModels
{
    public class MainViewModel : NotifyBaseModel
    {
        private ICommand _runAgainCommand, _exitCommand;

        private readonly MapTileControl _selectedMap;
        private readonly bool _isDebug;
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

        private Colors _winnerColor;
        public Colors WinnerColor
        {
            get => _winnerColor;
            set => SetValue(ref _winnerColor, value);
        }

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
            _isDebug = isDebug;
            _players = players;

            Initialize();
        }

        private void Initialize()
        {
            _control.MapArea.MapArea.Children.Clear();
            _gameManager = new GameManager(new Bitmap(_selectedMap.Map.UriSource.AbsolutePath), _players, _isDebug);
            _gameManager.OnGameOver += OnGameOver;
            _mapViewModel = new MapViewModel(_gameManager, _control.MapArea);
            LoadPlayers(_players);
        }

        public ICommand RunAgainCommand => _runAgainCommand ?? (_runAgainCommand = new DelegateCommand(RunAgain));
        public ICommand ExitCommand => _exitCommand ?? (_exitCommand = new DelegateCommand(_control.Close));

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

        public void StartGame()
        {
            GameStopped = false;
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
        }

        private void RunAgain()
        {
            Initialize();
            StartGame();
        }
    }
}
