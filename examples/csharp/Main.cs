using AntRunner.Interface;

namespace ExampleAnt
{
    public class Main : Ant
    {
        //Name of the ant
        public override string Name => "Cs Ant";
        //Resource string of icon file
        public override string FlagResource => "ExampleAnt.Flag.png";

        private int _mapWidth;
        private int _mapHeight;
        private ItemColor _myColor;

        private MapPosition[,] _map;
        private int _currentX;
        private int _currentY;
        private int _currentMode;
        private AntAction _lastAction;
        private AntAction _searchPrimary;
        private AntAction _searchSecondary;

        /// <summary>
        /// Initialize the ant with current map settings, runs once before each game starts.
        /// </summary>
        /// <param name="mapWidth">How many spaces wide is the current map.</param>
        /// <param name="mapHeight">How many spaces high is the current map.</param>
        /// <param name="color">Our ant's current color.</param>
        /// <param name="startX">X position in map where the ant starts.</param>
        /// <param name="startY">Y position in map where the ant starts.</param>
        public override void Initialize(int mapWidth, int mapHeight, ItemColor color, int startX, int startY)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _myColor = color;
            _currentX = startX;
            _currentY = startY;
            _currentMode = 0;
            _lastAction = AntAction.Wait;

            //Initialize internal map
            _map = new MapPosition[_mapWidth, _mapHeight];
            for (var y = 0; y < _mapHeight; ++y)
            {
                for (var x = 0; x < _mapWidth; ++x)
                {
                    _map[x, y] = new MapPosition();
                }
            }

            _map[_currentX, _currentY].Known = true;
            SetSearchDirection();
        }

        /// <summary>
        /// Runs every ~250ms for each turn if ant's thread is not still busy.
        /// </summary>
        /// <param name="state">Struct of the current state of the game, which includes any events that happen, locations of any ant with the flag, and the last turn's echo response if any.</param>
        public override void Tick(GameState state)
        {
            ProcessEcho(state.Response);
            ProcessGameEvent(state.Event);

            switch (_currentMode)
            {
                //Map mode
                case 0:
                    MapMode();
                    break;
            }
        }

        /// <summary>
        /// Sets the action for the current turn and sets a local _lastAction because the game will reset Action each turn to Wait and we lose what was there.
        /// </summary>
        /// <param name="a">The Action to set for the current turn.</param>
        private void SetAction(AntAction a)
        {
            //Set the action for the current turn.
            Action = a;
            //Set the last action so we know the direction on the next turn for successful moving and echo response.
            _lastAction = a;
        }

        /// <summary>
        /// Processes the EchoResponse object returned from previous echo action.
        /// </summary>
        /// <param name="response">Response item from game, if null then no echo was made previous turn.</param>
        private void ProcessEcho(EchoResponse response)
        {
            //There was no Echo last turn
            if (response == null) return;

            //Set map tiles
            switch (_lastAction)
            {
                case AntAction.EchoRight:
                    for (var i = 1; i < response.Distance; ++i)
                    {
                        _map[_currentX + i, _currentY].Known = true;
                    }

                    if (_currentX + response.Distance < _mapWidth)
                    {
                        _map[_currentX + response.Distance, _currentY].Known = true;
                        _map[_currentX + response.Distance, _currentY].Item = response.Item;
                    }
                    break;
                case AntAction.EchoDown:
                    for (var i = 1; i < response.Distance; ++i)
                    {
                        _map[_currentX, _currentY + i].Known = true;
                    }

                    if (_currentY + response.Distance < _mapHeight)
                    {
                        _map[_currentX, _currentY + response.Distance].Known = true;
                        _map[_currentX, _currentY + response.Distance].Item = response.Item;
                    }

                    break;
                case AntAction.EchoLeft:
                    for (var i = 1; i < response.Distance; ++i)
                    {
                        _map[_currentX - i, _currentY].Known = true;
                    }
                    if (_currentX - response.Distance >= 0)
                    {
                        _map[_currentX - response.Distance, _currentY].Known = true;
                        _map[_currentX - response.Distance, _currentY].Item = response.Item;
                    }
                    break;
                case AntAction.EchoUp:
                    for (var i = 1; i < response.Distance; ++i)
                    {
                        _map[_currentX, _currentY - i].Known = true;
                    }
                    if (_currentY - response.Distance >= 0)
                    {
                        _map[_currentX, _currentY - response.Distance].Known = true;
                        _map[_currentX, _currentY - response.Distance].Item = response.Item;
                    }
                    break;
                default:
                    //Should never come here
                    return;
            }
        }

        /// <summary>
        /// Process the result of the previous turn's events
        /// </summary>
        /// <param name="e">Flags of what events occurred. Multiple events can occur in one turn.</param>
        private void ProcessGameEvent(GameEvent e)
        {
            if (e.HasFlag(GameEvent.CollisionDamage))
            {
                //we ran into something
                return;
            }

            //Move was successful, update map position
            switch (_lastAction)
            {
                case AntAction.MoveRight:
                    _currentX += 1;
                    break;
                case AntAction.MoveDown:
                    _currentY += 1;
                    break;
                case AntAction.MoveLeft:
                    _currentX -= 1;
                    break;
                case AntAction.MoveUp:
                    _currentY -= 1;
                    break;
            }
        }


        /// <summary>
        /// Mode to randomly wonder around the map looking for the flag.
        /// </summary>
        private void MapMode()
        {
            //Check left
            if (_currentX > 0 && !_map[_currentX - 1, _currentY].Known)
            {
                SetAction(AntAction.EchoLeft);
                return;
            }

            //Check Right
            if (_currentX < _mapWidth - 1 && !_map[_currentX + 1, _currentY].Known)
            {
                SetAction(AntAction.EchoRight);
                return;
            }

            //Check Up
            if (_currentY > 0 && !_map[_currentX, _currentY - 1].Known)
            {
                SetAction(AntAction.EchoUp);
                return;
            }

            //Check Down
            if (_currentY < _mapHeight - 1 && !_map[_currentX, _currentY + 1].Known)
            {
                SetAction(AntAction.EchoDown);
                return;
            }

            //All tiles next to us are known, move a direction.
            if (CanMove(_searchPrimary))
            {
                SetAction(_searchPrimary);
                return;
            }

            if (CanMove(_searchSecondary))
            {
                SetAction(_searchSecondary);
                return;
            }

            //Can't move in that direction any more, try a different direction.
            if (_lastAction != _searchPrimary && CanMove(OppositeDirection(_searchPrimary)))
            {
                _searchPrimary = OppositeDirection(_searchPrimary);
                SetAction(_searchPrimary);
                return;
            }

            if (CanMove(OppositeDirection(_searchSecondary)))
            {
                _searchSecondary = OppositeDirection(_searchSecondary);
                SetAction(_searchSecondary);
                return;
            }

            //Can't find a direction, reset current search directions
            SetSearchDirection();
        }

        /// <summary>
        /// Try to guess a general direction to search in.
        /// </summary>
        private void SetSearchDirection()
        {
            if (_currentX == 0 || (decimal)_mapWidth / _currentX < 0.9m)
            {
                //search right
                _searchPrimary = AntAction.MoveRight;
            }
            else
            {
                //search left
                _searchPrimary = AntAction.MoveLeft;
            }

            if (_currentY == 0 || (decimal)_mapHeight / _currentY < 0.9m)
            {
                //search down
                _searchSecondary = AntAction.MoveDown;
            }
            else
            {
                //search up
                _searchSecondary = AntAction.MoveUp;
            }
        }

        /// <summary>
        /// Check if we can move to this space without running into anything.
        /// </summary>
        /// <param name="a">Action which brings us to the space.</param>
        /// <returns>True if safe.</returns>
        private bool CanMove(AntAction a)
        {
            MapPosition nextSpace;
            switch (a)
            {
                case AntAction.MoveRight:
                    if (_currentX + 1 == _mapWidth) return false;
                    nextSpace = _map[_currentX + 1, _currentY];
                    break;
                case AntAction.MoveDown:
                    if (_currentY + 1 == _mapHeight) return false;
                    nextSpace = _map[_currentX, _currentY + 1];
                    break;
                case AntAction.MoveLeft:
                    if (_currentX - 1 == -1) return false;
                    nextSpace = _map[_currentX - 1, _currentY];
                    break;
                case AntAction.MoveUp:
                    if (_currentY - 1 == -1) return false;
                    nextSpace = _map[_currentX, _currentY - 1];
                    break;
                default:
                    return true;
            }

            return nextSpace.Known && !Blocked(nextSpace.Item);
        }

        /// <summary>
        /// Checks if the item is something we can safely move onto.
        /// </summary>
        /// <param name="i">Item object that the space contains.</param>
        /// <returns>True if item is a blocking item.</returns>
        private bool Blocked(Item i)
        {
            switch (i)
            {
                case Item.SteelWall:
                case Item.BrickWall:
                case Item.Bomb:
                    return true;
                case Item.RedHome:
                    return _myColor != ItemColor.Red;
                case Item.BlueHome:
                    return _myColor != ItemColor.Blue;
                case Item.GreenHome:
                    return _myColor != ItemColor.Green;
                case Item.OrangeHome:
                    return _myColor != ItemColor.Orange;
                case Item.PinkHome:
                    return _myColor != ItemColor.Pink;
                case Item.YellowHome:
                    return _myColor != ItemColor.Yellow;
                case Item.GrayHome:
                    return _myColor != ItemColor.Gray;
                case Item.WhiteHome:
                    return _myColor != ItemColor.White;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Take in an action and return its opposite direction.
        /// </summary>
        /// <param name="a">Direction to get the opposite.</param>
        /// <returns>Opposite direction of action, Wait if no opposite exists.</returns>
        private static AntAction OppositeDirection(AntAction a)
        {
            switch (a)
            {
                case AntAction.MoveRight:
                    return AntAction.MoveLeft;
                case AntAction.MoveDown:
                    return AntAction.MoveUp;
                case AntAction.MoveLeft:
                    return AntAction.MoveRight;
                case AntAction.MoveUp:
                    return AntAction.MoveDown;
                case AntAction.EchoRight:
                    return AntAction.EchoLeft;
                case AntAction.EchoDown:
                    return AntAction.EchoUp;
                case AntAction.EchoLeft:
                    return AntAction.EchoRight;
                case AntAction.EchoUp:
                    return AntAction.EchoDown;
                case AntAction.ShootRight:
                    return AntAction.ShootLeft;
                case AntAction.ShootDown:
                    return AntAction.ShootUp;
                case AntAction.ShootLeft:
                    return AntAction.ShootRight;
                case AntAction.ShootUp:
                    return AntAction.ShootDown;
                default:
                    return AntAction.Wait;
            }
        }

        /// <summary>
        /// Struct object to represent each map coordinate
        /// </summary>
        public struct MapPosition
        {
            /// <summary>
            /// Boolean value of whether we have echoed this position yet
            /// </summary>
            public bool Known { get; set; }

            /// <summary>
            /// Item that this position holds
            /// </summary>
            public Item Item { get; set; }
        }
    }
}
