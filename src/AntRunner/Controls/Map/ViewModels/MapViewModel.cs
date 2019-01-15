using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using AntRunner.Controls.Ant.Views;
using AntRunner.Controls.Map.Views;
using AntRunner.Controls.Tiles.Views;
using AntRunner.Events;
using AntRunner.Interface;
using AntRunner.Models;

namespace AntRunner.Controls.Map.ViewModels
{
    public class MapViewModel : NotifyBaseModel
    {
        private readonly GameManager _gameManager;
        private readonly MapControl _mapControl;
        private readonly Dictionary<int, Dictionary<int, UIElement>> _tileLookup = new Dictionary<int, Dictionary<int, UIElement>>();

        public int MapWidth => _gameManager.MapWidth * 10;
        public int MapHeight => _gameManager.MapHeight * 10;

        public MapViewModel(GameManager gameManager, MapControl mapControl)
        {
            _gameManager = gameManager;
            _mapControl = mapControl;
            _mapControl.DataContext = this;
            DrawMap();
        }

        public void AddPlayer(AntWrapper ant)
        {
            _mapControl.MapArea.Children.Add(new PlayerControl(ant));
            ant.ShootEventHandler -= AntOnShootEventHandler;
            ant.ShootEventHandler += AntOnShootEventHandler;
        }

        public void ClearPlayer(AntWrapper ant)
        {
            ant.ShootEventHandler -= AntOnShootEventHandler;
            ant.CurrentTile = null;
        }

        private void AntOnShootEventHandler(object sender, ShootEventHandler e)
        {
            if (!(sender is AntWrapper ant)) return;
            Application.Current.Dispatcher.Invoke(() =>
            {
                _mapControl.MapArea.Children.Insert(0, new ShootControl(ant.CurrentTile) {ShotDistance = e.Distance, Direction = e.Direction});
            });
        }

        private void DrawMap()
        {
            foreach (var t in _gameManager.Map)
            {
                t.PropertyChanged += TileOnPropertyChanged;
                SetTile(t);
            }
        }

        private void TileOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!e.PropertyName.Equals(nameof(MapTile.Item))) return;
            SetTile((MapTile)sender);
        }

        private void SetTile(MapTile t)
        {
            Application.Current.Dispatcher.Invoke(() =>
                {
                    switch (t.Item)
                    {
                        case Items.Nothing:
                            this[t.X, t.Y] = null;
                            return;
                        case Items.SteelWall:
                            this[t.X, t.Y] = new SteelWallControl(t);
                            break;
                        case Items.BrickWall:
                            this[t.X, t.Y] = new BrickWallControl(t);
                            break;
                        case Items.Flag:
                            this[t.X, t.Y] = new FlagControl(t);
                            break;
                        case Items.PowerUpShield:
                            this[t.X, t.Y] = new ShieldKitControl(t);
                            break;
                        case Items.PowerUpBomb:
                            this[t.X, t.Y] = new BombKitControl(t);
                            break;
                        case Items.PowerUpHealth:
                            this[t.X, t.Y] = new HealthKitControl(t);
                            break;
                        case Items.Bomb:
                            this[t.X, t.Y] = new BombControl(t);
                            break;
                        case Items.RedHome:
                            this[t.X, t.Y] = new HomeControl(Colors.Red, t);
                            break;
                        case Items.BlueHome:
                            this[t.X, t.Y] = new HomeControl(Colors.Blue, t);
                            break;
                        case Items.GreenHome:
                            this[t.X, t.Y] = new HomeControl(Colors.Green, t);
                            break;
                        case Items.OrangeHome:
                            this[t.X, t.Y] = new HomeControl(Colors.Orange, t);
                            break;
                        case Items.PinkHome:
                            this[t.X, t.Y] = new HomeControl(Colors.Pink, t);
                            break;
                        case Items.YellowHome:
                            this[t.X, t.Y] = new HomeControl(Colors.Yellow, t);
                            break;
                        case Items.GrayHome:
                            this[t.X, t.Y] = new HomeControl(Colors.Gray, t);
                            break;
                        case Items.WhiteHome:
                            this[t.X, t.Y] = new HomeControl(Colors.White, t);
                            break;
                    }
                }
            );
        }

        public UIElement this[int x, int y]
        {
            get
            {
                if (!_tileLookup.ContainsKey(y)) return null;
                if (!_tileLookup[y].ContainsKey(x)) return null;
                return _tileLookup[y][x];
            }
            set
            {
                if (!_tileLookup.ContainsKey(y))
                {
                    _tileLookup.Add(y, new Dictionary<int, UIElement>());
                }
                if (!_tileLookup[y].ContainsKey(x))
                {
                    if (value == null) return;

                    _tileLookup[y].Add(x, value);
                    _mapControl.MapArea.Children.Insert(0, value);
                }
                else
                {
                    _mapControl.MapArea.Children.Remove(_tileLookup[y][x]);
                    if (value != null)
                    {
                        _mapControl.MapArea.Children.Insert(0, value);
                    }
                    _tileLookup[y][x] = value;
                }
            }
        }
    }
}
