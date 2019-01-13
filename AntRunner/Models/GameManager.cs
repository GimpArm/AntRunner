using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Timers;
using AntRunner.Events;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public class GameManager
    {
        public readonly int MapHeight;
        public readonly int MapWidth;
        private readonly Timer _gameTicker = new Timer(250);
        private readonly Dictionary<Colors, GameEvent> _eventStack = new Dictionary<Colors, GameEvent>();
        private readonly Dictionary<Colors, MapTile> _echoStack = new Dictionary<Colors, MapTile>();
        private readonly bool _isDebug;
        private long _currentTick;
        private int _startPlayer;
        public Map Map { get; }

        private bool _hasFlag;
        private MapTile _flagCarrierDiedTile;
        private AntWrapper _antWithFlag;

        public Dictionary<Colors, AntWrapper> Players { get; }
        
        public bool GameRunning { get; private set; }
        public EventHandler<GameOverEventArgs> OnGameOver;

        public GameManager(Bitmap mapDefinition, IList<AntWrapper> players, bool isDebug)
        {
            _isDebug = isDebug;
            MapHeight = mapDefinition.Height;
            MapWidth = mapDefinition.Width;
            Players = players.ToDictionary(k => k.Color, v => v);
            Map = new Map(mapDefinition, Players.Keys.ToList());
            _gameTicker.Elapsed += GameTickerOnElapsed;
        }
        
        public void Start()
        {
            foreach (var player in Players.Values)
            {
                var tile = Map.RandomTile();
                try
                {
                    player.Initialize(Map, tile);
                }
                catch
                {
                    //TODO: errors
                }
            }

            GameRunning = true;
            _gameTicker.Start();
        }

        private void GameTickerOnElapsed(object sender, ElapsedEventArgs e)
        {
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
                _gameTicker.Stop();
                if (_hasFlag && _antWithFlag != null)
                {
                    OnGameOver?.Invoke(this, new GameOverEventArgs(_antWithFlag.Color, _antWithFlag.Name));
                }
                else
                {
                    OnGameOver?.Invoke(this, new GameOverEventArgs());
                }
                _eventStack.Clear();
                _echoStack.Clear();
                return;
            }

            if (_hasFlag && _antWithFlag != null)
            {
                SetTicks(new GameState(_currentTick, true,  _antWithFlag.CurrentTile?.X ?? _flagCarrierDiedTile.X, _antWithFlag.CurrentTile?.Y ?? _flagCarrierDiedTile.Y, _antWithFlag.Color));
            }
            else
            {
                SetTicks(new GameState(_currentTick));
            }

            if (_flagCarrierDiedTile != null)
            {

                _hasFlag = false;
                _flagCarrierDiedTile.Item |= Items.Flag;
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
        }

        private int GetActions()
        {
            var deadCount = 0;
            foreach (var kvp in Players.Where(x => x.Value != null).ToDictionary(x => x.Value, y => y.Value.GetAction()).OrderBy(x => x.Value))
            {
                var current = kvp.Key;
                if (current.Health == 0)
                {
                    deadCount++;
                }
                switch (kvp.Value)
                {
                    case Actions.Wait:
                        break;
                    case Actions.MoveRight:
                        current.Direction = ActionToDirection(kvp.Value);
                        SetEvent(current, Map.MoveTo(current.CurrentTile.X + 1, current.CurrentTile.Y, current, SetEvent));
                        break;
                    case Actions.MoveDown:
                        current.Direction = ActionToDirection(kvp.Value);
                        SetEvent(current, Map.MoveTo(current.CurrentTile.X, current.CurrentTile.Y + 1, current, SetEvent));
                        break;
                    case Actions.MoveLeft:
                        current.Direction = ActionToDirection(kvp.Value);
                        SetEvent(current, Map.MoveTo(current.CurrentTile.X - 1, current.CurrentTile.Y, current, SetEvent));
                        break;
                    case Actions.MoveUp:
                        current.Direction = ActionToDirection(kvp.Value);
                        SetEvent(current, Map.MoveTo(current.CurrentTile.X, current.CurrentTile.Y - 1, current, SetEvent));
                        break;
                    case Actions.EchoRight:
                    case Actions.EchoDown:
                    case Actions.EchoLeft:
                    case Actions.EchoUp:
                        current.Direction = ActionToDirection(kvp.Value);
                        _echoStack.Add(current.Color, Map.GetTileTo(current, kvp.Value));
                        break;
                    case Actions.ShieldOn:
                        current.ShieldsOn = true;
                        break;
                    case Actions.ShieldOff:
                        current.ShieldsOn = false;
                        break;
                    case Actions.DropBomb:
                        if (current.Bombs == 0) break;
                        current.Bombs--;
                        current.CurrentTile.Item |= Items.Bomb;
                        break;
                    case Actions.ShootUp:
                    case Actions.ShootLeft:
                    case Actions.ShootDown:
                    case Actions.ShootRight:
                        current.Direction = ActionToDirection(kvp.Value);
                        ProcessShot(current, kvp.Value);
                        break;
                }
            }

            return deadCount;
        }

        private void ProcessEvents()
        {
            var playerEnumerator = NextPlayer();
            while (playerEnumerator.MoveNext() && playerEnumerator.Current != null)
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
                    current.Health += ItemBonus.Health;
                }
                if (events.HasFlag(GameEvent.PickUpShield))
                {
                    current.Shields += ItemBonus.Shield;
                }
                if (events.HasFlag(GameEvent.PickUpBomb))
                {
                    current.Bombs += ItemBonus.Bomb;
                }
                if (events.HasFlag(GameEvent.PickUpFlag))
                {
                    _hasFlag = true;
                    _antWithFlag = current;
                    current.HasFlag = true;
                }
                if (events.HasFlag(GameEvent.BombDamage))
                {
                    current.Damage(Damage.Bomb);
                }

                if (events.HasFlag(GameEvent.ShotDamageDown) || events.HasFlag(GameEvent.ShotDamageLeft) || events.HasFlag(GameEvent.ShotDamageRight) || events.HasFlag(GameEvent.ShotDamageUp))
                {
                    current.Damage(Damage.Shot);
                }
                if (events.HasFlag(GameEvent.CollisionDamage))
                {
                    current.Damage(Damage.Collision);
                }
                if (events.HasFlag(GameEvent.ImpactDamageDown) || events.HasFlag(GameEvent.ImpactDamageLeft) || events.HasFlag(GameEvent.ImpactDamageRight) || events.HasFlag(GameEvent.ImpactDamageUp))
                {
                    current.Damage(Damage.Impact);
                }

                if (current.Health == 0)
                {
                    _eventStack[current.Color] = GameEvent.Dead;
                    if (_hasFlag && _antWithFlag != null && _antWithFlag.Color == current.Color)
                    {
                        _flagCarrierDiedTile = current.CurrentTile;
                    }
                    current.CurrentTile = null;
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
                playerEnumerator.Current.Tick(new GameState(state, events, GetEcho(current)), _isDebug);
            }
        }

        private EchoResponse GetEcho(AntWrapper ant)
        {
            if (!_echoStack.ContainsKey(ant.Color)) return null;
            var tile = _echoStack[ant.Color];
            if (tile == null) return null;

            return new EchoResponse(Utilities.CalculateDistance(ant, tile), tile.Item);
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

        private void ProcessShot(AntWrapper ant, Actions action)
        {
            var tile = Map.GetTileTo(ant, action);
            if (tile != null && tile.X >= 0 && tile.X < MapWidth && tile.Y >= 0 && tile.Y < MapHeight)
            {
                if (tile.OccupiedBy != null)
                {
                    SetEvent(tile.OccupiedBy, Utilities.ShotDirectionToEvent(action));
                }
                else if (tile.Item != Items.Nothing)
                {
                    if (tile.Item.HasFlag(Items.BrickWall))
                    {
                        BreakWall(tile);
                    }
                    else if (tile.Item.HasFlag(Items.Bomb) || tile.Item.HasFlag(Items.PowerUpBomb) || tile.Item.HasFlag(Items.PowerUpHealth) || tile.Item.HasFlag(Items.PowerUpShield))
                    {
                        tile.Item = Items.Nothing;
                    }
                }
                ant.Shoot(tile);
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
                tile.Item = Items.Nothing;
            }
            else if (result < 80)
            {
                tile.Item = Items.PowerUpBomb;
            }
            else if (result < 90)
            {
                tile.Item = Items.PowerUpHealth;
            }
            else if (result < 100)
            {
                tile.Item = Items.PowerUpShield;
            }
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

        private static Direction ActionToDirection(Actions action)
        {
            switch (action)
            {
                case Actions.ShootRight:
                case Actions.EchoRight:
                case Actions.MoveRight:
                    return Direction.Right;
                case Actions.ShootDown:
                case Actions.EchoDown:
                case Actions.MoveDown:
                    return Direction.Down;
                case Actions.ShootLeft:
                case Actions.EchoLeft:
                case Actions.MoveLeft:
                    return Direction.Left;
                case Actions.ShootUp:
                case Actions.EchoUp:
                case Actions.MoveUp:
                    return Direction.Up;
            }

            return Direction.Up;
        }
    }
}
