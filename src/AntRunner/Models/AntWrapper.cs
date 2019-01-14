using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AntRunner.Events;
using AntRunner.Interface;
using Colors = AntRunner.Interface.Colors;

namespace AntRunner.Models
{
    public class AntWrapper : NotifyBaseModel
    {
        public event EventHandler<ShootEventHandler> ShootEventHandler;
        private readonly BackgroundWorker _antWorkerThread = new BackgroundWorker();
        public string Name => Ant.Name;

        public Ant Ant { get; }
        public Items AntItem { get; private set; }
        public Items AntHome { get; private set; }
        public Actions LastAction { get; private set; }

        public ImageSource Icon { get; }

        private Colors _color;
        public Colors Color
        {
            get => _color;
            private set => SetValue(ref _color, value);
        }

        private bool _hasFlag;
        public bool HasFlag
        {
            get => _hasFlag;
            set => SetValue(ref _hasFlag, value);
        }

        private MapTile _currentTile;
        public MapTile CurrentTile
        {
            get => _currentTile;
            set
            {
                if (_currentTile != null)
                {
                    _currentTile.Item &= ~AntItem;
                    _currentTile.OccupiedBy = null;
                }
                if (value != null)
                {
                    value.Item |= AntItem;
                    value.OccupiedBy = this;
                }
                SetValue(ref _currentTile, value);
            }
        }

        private Direction _direction = Direction.Up;
        public Direction Direction
        {
            get => _direction;
            set => SetValue(ref _direction, value);
        }

        private bool _shieldsOn;
        public bool ShieldsOn
        {
            get => _shieldsOn;
            set => SetValue(ref _shieldsOn, value);
        }

        private int _health = 100;
        public int Health
        {
            get => _health;
            set
            {
                if (value > 100)
                    value = 100;
                else if (value < 0)
                    value = 0;
                SetValue(ref _health, value);
            }
        }

        public int ShieldCounter { get; set; }

        private int _shields = 50;
        public int Shields
        {
            get => _shields;
            set
            {
                if (value > 100)
                    value = 100;
                else if (value < 0)
                    value = 0;
                if (value == 0)
                    ShieldsOn = false;
                SetValue(ref _shields, value);
            }
        }

        private int _bombs;
        public int Bombs
        {
            get => _bombs;
            set => SetValue(ref _bombs, value);
        }

        public AntWrapper(Ant ant, Colors color)
        {
            Ant = ant;
            SetColor(color);
            _antWorkerThread.DoWork += AntWorkerThreadOnDoWork;

            try
            {
                Icon = BitmapFrame.Create(ant.Icon, BitmapCreateOptions.None, BitmapCacheOption.Default);
            }
            catch
            {
                //Do Nothing
            }
        }

        public void SetColor(Colors color)
        {
            Color = color;
            AntItem = Utilities.ColorToAntItem(Color);
            AntHome = Utilities.ColorToAntHomeItem(color);
        }

        public void Initialize(Map map, MapTile currentTile)
        {
            Shields = 50;
            Health = 100;
            Bombs = 0;
            ShieldsOn = false;
            HasFlag = false;
            CurrentTile = currentTile;
            Ant.Initialize(map.Width, map.Height, Color, CurrentTile.X, CurrentTile.Y);
        }

        private void AntWorkerThreadOnDoWork(object sender, DoWorkEventArgs e)
        {
            if (Health == 0) return;
            Ant.Tick((GameState)e.Argument);
        }

        public void Damage(int value)
        {
            if (value == 0) return;
            if (ShieldsOn)
            {
                if (Shields >= value)
                {
                    Shields -= value;
                    value = 0;
                }
                else
                {
                    value = value - Shields;
                    Shields = 0;
                }
            }
            if (value == 0) return;
            if (value >= Health)
            {
                Health = 0;
            }
            else
            {
                Health -= value;
            }
        }

        public Actions GetAction()
        {
            LastAction = Ant.Action;
            Ant.Action = Actions.Wait;
            return LastAction;
        }

        public void Tick(GameState state, bool isDebug)
        {
            if (_antWorkerThread.IsBusy) return;
            try
            {
                _antWorkerThread.RunWorkerAsync(state);
                if (!isDebug) return;
                while (_antWorkerThread.IsBusy)
                {
                    Task.Delay(10).Wait();
                }
            }
            catch
            {
                //TODO: Error handling
            }
        }

        public void Shoot(MapTile tile)
        {
            ShootEventHandler?.Invoke(this, new ShootEventHandler(Utilities.CalculateDistance(this, tile), Direction));
        }

        public void Shoot()
        {
            switch (Direction)
            {
                case Direction.Right:
                    ShootEventHandler?.Invoke(this, new ShootEventHandler(99 - CurrentTile.X, Direction));
                    break;
                case Direction.Down:
                    ShootEventHandler?.Invoke(this, new ShootEventHandler(99 - CurrentTile.Y, Direction));
                    break;
                case Direction.Left:
                    ShootEventHandler?.Invoke(this, new ShootEventHandler(CurrentTile.X, Direction));
                    break;
                case Direction.Up:
                    ShootEventHandler?.Invoke(this, new ShootEventHandler(CurrentTile.Y, Direction));
                    break;
            }
            
        }
    }
}
