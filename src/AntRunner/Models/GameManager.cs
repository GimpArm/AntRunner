using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Timers;
using System.Windows;
using AntRunner.Events;
using AntRunner.Extensions;
using AntRunner.Game.Interface;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public class GameManager : IDisposable
    {
        public readonly int MapHeight;
        public readonly int MapWidth;
        private readonly Timer _gameTicker = new Timer(250);
        private readonly Dictionary<ItemColor, GameEvent> _eventStack = new Dictionary<ItemColor, GameEvent>();
        private readonly Dictionary<ItemColor, MapTile> _echoStack = new Dictionary<ItemColor, MapTile>();
        private long _currentTick;
        private int _startPlayer;
        public Map Map { get; }
        public bool IsDebug { get; set; }

        private bool _hasFlag;
        private MapTile _flagCarrierDiedTile;
        private AntWrapper _antWithFlag;

        private Guid _currentGameID;
        private List<IExternalComponent> _externalComponentList = new List<IExternalComponent>();
        public List<IExternalComponent> ExternalComponents => _externalComponentList;

        public Dictionary<ItemColor, AntWrapper> Players { get; }

        public bool GameRunning { get; private set; }
        public EventHandler<GameOverEventArgs> OnGameOver;
        public EventHandler<ExplosionEventArgs> OnExplosion;

        #region Member - CurrentState
        private GameRunningModeType _currentRunningMode = GameRunningModeType.Playing;
        public event EventHandler OnRunningModeChanged;
        public GameRunningModeType CurrentRunningMode
        {
            get => _currentRunningMode;
            set
            {
                if (_currentRunningMode != value)
                {
                    _currentRunningMode = value;
                    OnRunningModeChanged?.Invoke(value, null);
                }
            }
        } 
        #endregion

        public List<IGameEventHook> GameHookList => ExternalComponents.Where(E => E.IsActiv).Select(H => H.GameEventHook).ToList();

        public GameManager(IEnumerable<AntWrapper> players, bool isDebug, Bitmap mapDefinition = null)
        {
            _currentGameID = Guid.NewGuid();

            IsDebug = isDebug;
            Players = players.ToDictionary(k => k.Color, v => v);
            if (mapDefinition != null)
            {
                Map = new Map(mapDefinition, Players.Keys.ToList());
            }
            else
            {
                Map = new Map(Players.Keys.ToList());
            }
            MapHeight = Map.Height;
            MapWidth = Map.Width;
            _gameTicker.Elapsed += GameTickerOnElapsed;

            FindAndStartExternalComponents();

            GameHookList.ForEach(H =>
            {
                H.CreateGame(_currentGameID);
                H.SetMap(mapDefinition.ToByteArray());
            });
        }

        private void FindAndStartExternalComponents()
        {
            var externalAssemblys = new Assembly[]
            {
                Assembly.Load("AntRunner.ExternalComponent.LoggerWithUI")
            };

            var allTypes = externalAssemblys
                .SelectMany(S => S.DefinedTypes)
                .Where(C => C.ImplementedInterfaces.Contains(typeof(IExternalComponent))).ToList();

            allTypes.ForEach(T =>
            {
                var newInstance = (IExternalComponent)Activator.CreateInstance(T);
                if (newInstance != null)
                {
                    _externalComponentList.Add(newInstance);
                }
            });
        }

        public void Start()
        {
            foreach (var player in Players.Values)
            {
                player.Initialize(Map, Map.RandomTile());
            }

            GameRunning = true;
            _gameTicker.Start();

            GameHookList.ForEach(H => H.StartGame(_currentGameID));
        }

        public void Stop()
        {
            _gameTicker.Stop();
            GameRunning = false;
            _eventStack.Clear();
            _echoStack.Clear();

            GameHookList.ForEach(H => H.StopGame(_currentGameID));
            _externalComponentList.ForEach(C => C.Stop());
        }

        private void GameTickerOnElapsed(object sender, ElapsedEventArgs e)
        {
            if (_currentRunningMode == GameRunningModeType.Pause)
            { 
                return;
            }

            _gameTicker.Stop();
            _currentTick++;
            var deadCount = GetActions();
            if (deadCount == Players.Count)
            {
                GameRunning = false;
            }
            else
            {
                ProcessEvents();
            }
            if (!GameRunning)
            {
                Stop();
                if (_hasFlag && _antWithFlag != null)
                {
                    OnGameOver?.Invoke(this, new GameOverEventArgs(_antWithFlag.Color, _antWithFlag.Name));
                }
                else
                {
                    OnGameOver?.Invoke(this, new GameOverEventArgs());
                }
                return;
            }

            if (_hasFlag && _antWithFlag != null)
            {
                SetTicks(new GameState(_currentTick, _antWithFlag.CurrentTile?.X ?? _flagCarrierDiedTile.X, _antWithFlag.CurrentTile?.Y ?? _flagCarrierDiedTile.Y, _antWithFlag.Color));
            }
            else
            {
                SetTicks(new GameState(_currentTick));
            }

            if (_flagCarrierDiedTile != null)
            {

                _hasFlag = false;
                _flagCarrierDiedTile.Item = Item.Flag;
                _antWithFlag = null;
                _flagCarrierDiedTile = null;
            }
            _gameTicker.Start();

            _startPlayer++;
            if (_startPlayer > Players.Count)
            {
                _startPlayer = 0;
            }
            _eventStack.Clear();
            _echoStack.Clear();

            if (_currentRunningMode == GameRunningModeType.NextStep)
            {
                CurrentRunningMode = GameRunningModeType.Pause;
            }
        }

        private int GetActions()
        {
            var deadCount = 0;
            foreach (var kvp in Players.Where(x => x.Value != null).ToDictionary(x => x.Value, y => y.Value.GetAction()).OrderBy(x => x.Value))
            {
                try
                {
                    var current = kvp.Key;
                    if (current.Health == 0)
                    {
                        deadCount++;
                    }
                    switch (kvp.Value)
                    {
                        case AntAction.Wait:
                            break;
                        case AntAction.MoveRight:
                            current.Direction = ActionToDirection(kvp.Value);
                            SetEvent(current, Map.MoveTo(current.CurrentTile.X + 1, current.CurrentTile.Y, current, SetEvent));
                            break;
                        case AntAction.MoveDown:
                            current.Direction = ActionToDirection(kvp.Value);
                            SetEvent(current, Map.MoveTo(current.CurrentTile.X, current.CurrentTile.Y + 1, current, SetEvent));
                            break;
                        case AntAction.MoveLeft:
                            current.Direction = ActionToDirection(kvp.Value);
                            SetEvent(current, Map.MoveTo(current.CurrentTile.X - 1, current.CurrentTile.Y, current, SetEvent));
                            break;
                        case AntAction.MoveUp:
                            current.Direction = ActionToDirection(kvp.Value);
                            SetEvent(current, Map.MoveTo(current.CurrentTile.X, current.CurrentTile.Y - 1, current, SetEvent));
                            break;
                        case AntAction.EchoRight:
                        case AntAction.EchoDown:
                        case AntAction.EchoLeft:
                        case AntAction.EchoUp:
                            current.Direction = ActionToDirection(kvp.Value);
                            _echoStack.Add(current.Color, Map.GetTileTo(current, kvp.Value));
                            break;
                        case AntAction.ShieldOn:
                            current.ShieldsOn = true;
                            break;
                        case AntAction.ShieldOff:
                            current.ShieldsOn = false;
                            break;
                        case AntAction.DropBomb:
                            if (current.Bombs == 0) break;
                            current.Bombs--;
                            current.CurrentTile.Item = Item.Bomb;
                            break;
                        case AntAction.ShootUp:
                        case AntAction.ShootLeft:
                        case AntAction.ShootDown:
                        case AntAction.ShootRight:
                            current.Direction = ActionToDirection(kvp.Value);
                            ProcessShot(current, kvp.Value);
                            break;
                    }

                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        GameHookList.ForEach(H => H.SetPlayerAction(new AntState {
                            ID = current.ID,
                            Name = current.Name,
                            Color = current.Color,
                            PositionX = current.CurrentTile?.X,
                            PositionY = current.CurrentTile?.Y,
                            LastAction = current.LastAction,
                            Health = current.Health,
                            Shields = current.Shields,
                        }));
                    }));
                }
                catch
                {
                    //Do Nothing
                }
            }

            return deadCount;
        }

        private void ProcessEvents()
        {
            var playerEnumerator = NextPlayer();
            while (playerEnumerator.MoveNext() && playerEnumerator.Current != null)
            {
                try
                {
                    var current = playerEnumerator.Current;
                    if (!_eventStack.ContainsKey(current.Color)) continue;
                    var events = _eventStack[playerEnumerator.Current.Color];
                    if (current.ShieldsOn)
                    {
                        current.ShieldCounter++;
                        if (current.ShieldCounter >= 4)
                        {
                            current.Shields--;
                            current.ShieldCounter = 0;
                        }
                    }

                    if (events.HasFlag(GameEvent.GameOver))
                    {
                        GameRunning = false;
                        return;
                    }

                    if (events.HasFlag(GameEvent.PickUpHealth))
                    {
                        current.Health += ItemBonusValues.Health;
                    }
                    if (events.HasFlag(GameEvent.PickUpShield))
                    {
                        current.Shields += ItemBonusValues.Shield;
                    }
                    if (events.HasFlag(GameEvent.PickUpBomb))
                    {
                        current.Bombs += ItemBonusValues.Bomb;
                    }
                    if (events.HasFlag(GameEvent.PickUpFlag))
                    {
                        _hasFlag = true;
                        _antWithFlag = current;
                        current.HasFlag = true;
                    }
                    if (events.HasFlag(GameEvent.BombDamage))
                    {
                        current.Damage(DamageValues.Bomb);
                        OnExplosion?.Invoke(this, new ExplosionEventArgs(current.CurrentTile));
                    }

                    if (events.HasFlag(GameEvent.ShotDamageDown) || events.HasFlag(GameEvent.ShotDamageLeft) || events.HasFlag(GameEvent.ShotDamageRight) || events.HasFlag(GameEvent.ShotDamageUp))
                    {
                        current.Damage(DamageValues.Shot);
                    }
                    if (events.HasFlag(GameEvent.CollisionDamage))
                    {
                        current.Damage(DamageValues.Collision);
                    }
                    if (events.HasFlag(GameEvent.ImpactDamageDown) || events.HasFlag(GameEvent.ImpactDamageLeft) || events.HasFlag(GameEvent.ImpactDamageRight) || events.HasFlag(GameEvent.ImpactDamageUp))
                    {
                        current.Damage(DamageValues.Impact);
                    }

                    if (current.Health == 0)
                    {
                        _eventStack[current.Color] = GameEvent.Dead;
                        if (_hasFlag && _antWithFlag != null && _antWithFlag.Color == current.Color)
                        {
                            _flagCarrierDiedTile = current.CurrentTile;
                            current.HasFlag = false;
                        }
                        current.CurrentTile = null;
                    }
                }
                catch
                {
                    //Do Nothing
                }
            }
        }

        private void SetTicks(GameState state)
        {
            var playerEnumerator = NextPlayer();
            while (playerEnumerator.MoveNext() && playerEnumerator.Current != null)
            {
                var current = playerEnumerator.Current;
                if (current.Health == 0) continue;

                var events = _eventStack.ContainsKey(current.Color) ? _eventStack[current.Color] : GameEvent.Nothing;
                playerEnumerator.Current.Tick(new GameState(state, events, GetEcho(current)), IsDebug);
            }
        }

        private EchoResponse GetEcho(AntWrapper ant)
        {
            if (!_echoStack.ContainsKey(ant.Color)) return null;
            var tile = _echoStack[ant.Color];
            if (tile == null) return null;

            return new EchoResponse(Utilities.CalculateDistance(ant, tile), tile.OccupiedBy?.AntItem ?? tile.Item);
        }


        private void SetEvent(AntWrapper ant, GameEvent @event)
        {
            if (!_eventStack.ContainsKey(ant.Color))
            {
                _eventStack.Add(ant.Color, @event);
            }
            else
            {
                _eventStack[ant.Color] |= @event;
            }
        }

        private void ProcessShot(AntWrapper ant, AntAction action)
        {
            var tile = Map.GetTileTo(ant, action);
            if (tile != null && tile.X >= 0 && tile.X < MapWidth && tile.Y >= 0 && tile.Y < MapHeight)
            {
                if (tile.OccupiedBy != null)
                {
                    SetEvent(tile.OccupiedBy, Utilities.ShotDirectionToEvent(action));
                }
                else
                {
                    switch (tile.Item)
                    {
                        case Item.BrickWall:
                            BreakWall(tile);
                            break;
                        case Item.Bomb:
                        case Item.PowerUpBomb:
                        case Item.PowerUpHealth:
                        case Item.PowerUpShield:
                            tile.Item = Item.Empty;
                            break;
                    }
                }
                ant.Shoot(tile);
                OnExplosion?.Invoke(this, new ExplosionEventArgs(tile));
            }
            else
            {
                ant.Shoot();
            }
        }

        private static void BreakWall(MapTile tile)
        {
            var result = Utilities.Random.Next(0, 99);
            if (result < 70)
            {
                tile.Item = Item.Empty;
            }
            else if (result < 80)
            {
                tile.Item = Item.PowerUpBomb;
            }
            else if (result < 90)
            {
                tile.Item = Item.PowerUpHealth;
            }
            else if (result < 100)
            {
                tile.Item = Item.PowerUpShield;
            }
            tile.Redraw();
        }

        private IEnumerator<AntWrapper> NextPlayer()
        {
            foreach (var player in Players.Values.Skip(_startPlayer))
            {
                yield return player;
            }

            foreach (var player in Players.Values.Take(_startPlayer))
            {
                yield return player;
            }
        }

        private static Direction ActionToDirection(AntAction action)
        {
            switch (action)
            {
                case AntAction.ShootRight:
                case AntAction.EchoRight:
                case AntAction.MoveRight:
                    return Direction.Right;
                case AntAction.ShootDown:
                case AntAction.EchoDown:
                case AntAction.MoveDown:
                    return Direction.Down;
                case AntAction.ShootLeft:
                case AntAction.EchoLeft:
                case AntAction.MoveLeft:
                    return Direction.Left;
                case AntAction.ShootUp:
                case AntAction.EchoUp:
                case AntAction.MoveUp:
                    return Direction.Up;
            }

            return Direction.Up;
        }

        public void Dispose()
        {
            Stop();
            _gameTicker?.Dispose();
        }
    }
}
