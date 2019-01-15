using AntRunner.Interface;

namespace ExampleAnt
{
    public class Main : Ant
    {
        //Name of the ant
        public override string Name => "Example Ant";
        //Resource string of icon file
        public override string IconResource => "ExampleAnt.Icon.png";

        private int _mapWidth;
        private int _mapHeight;
        private Colors _myColor;

        private MapPosition[,] _map;
        private int _currentX;
        private int _currentY;
        private int _currentMode;
        private Actions _lastAction;
        private Actions _searchPrimary;
        private Actions _searchSecondary;

        /// <summary>
        /// Initialize the ant with current map settings, runs once before each game starts.
        /// </summary>
        /// <param name="mapWidth">How many spaces wide is the current map.</param>
        /// <param name="mapHeight">How many spaces high is the current map.</param>
        /// <param name="color">Our ant's current color.</param>
        /// <param name="startX">X position in map where the ant starts.</param>
        /// <param name="startY">Y position in map where the ant starts.</param>
        public override void Initialize(int mapWidth, int mapHeight, Colors color, int startX, int startY)
        {
            _mapWidth = mapWidth;
            _mapHeight = mapHeight;
            _myColor = color;
            _currentX = startX;
            _currentY = startY;
            _currentMode = 0;
            _lastAction = Actions.Wait;

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
        private void SetAction(Actions a)
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
                case Actions.EchoRight:
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
                case Actions.EchoDown:
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
                case Actions.EchoLeft:
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
                case Actions.EchoUp:
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
                case Actions.MoveRight:
                    _currentX += 1;
                    break;
                case Actions.MoveDown:
                    _currentY += 1;
                    break;
                case Actions.MoveLeft:
                    _currentX -= 1;
                    break;
                case Actions.MoveUp:
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
                SetAction(Actions.EchoLeft);
                return;
            }

            //Check Right
            if (_currentX < _mapWidth - 1 && !_map[_currentX + 1, _currentY].Known)
            {
                SetAction(Actions.EchoRight);
                return;
            }

            //Check Up
            if (_currentY > 0 && !_map[_currentX, _currentY - 1].Known)
            {
                SetAction(Actions.EchoUp);
                return;
            }

            //Check Down
            if (_currentY < _mapHeight - 1 && !_map[_currentX, _currentY + 1].Known)
            {
                SetAction(Actions.EchoDown);
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
                _searchPrimary = Actions.MoveRight;
            }
            else
            {
                //search left
                _searchPrimary = Actions.MoveLeft;
            }

            if (_currentY == 0 || (decimal)_mapHeight / _currentY < 0.9m)
            {
                //search down
                _searchSecondary = Actions.MoveDown;
            }
            else
            {
                //search up
                _searchSecondary = Actions.MoveUp;
            }
        }

        /// <summary>
        /// Check if we can move to this space without running into anything.
        /// </summary>
        /// <param name="a">Action which brings us to the space.</param>
        /// <returns>True if safe.</returns>
        private bool CanMove(Actions a)
        {
            MapPosition nextSpace;
            switch (a)
            {
                case Actions.MoveRight:
                    if (_currentX + 1 == _mapWidth) return false;
                    nextSpace = _map[_currentX + 1, _currentY];
                    break;
                case Actions.MoveDown:
                    if (_currentY + 1 == _mapHeight) return false;
                    nextSpace = _map[_currentX, _currentY + 1];
                    break;
                case Actions.MoveLeft:
                    if (_currentX - 1 == -1) return false;
                    nextSpace = _map[_currentX - 1, _currentY];
                    break;
                case Actions.MoveUp:
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
        /// <param name="i">Items object that the space contains.</param>
        /// <returns>True if item is a blocking item.</returns>
        private bool Blocked(Items i)
        {
            switch (i)
            {
                case Items.SteelWall:
                case Items.BrickWall:
                case Items.Bomb:
                    return true;
                case Items.RedHome:
                    return _myColor != Colors.Red;
                case Items.BlueHome:
                    return _myColor != Colors.Blue;
                case Items.GreenHome:
                    return _myColor != Colors.Green;
                case Items.OrangeHome:
                    return _myColor != Colors.Orange;
                case Items.PinkHome:
                    return _myColor != Colors.Pink;
                case Items.YellowHome:
                    return _myColor != Colors.Yellow;
                case Items.GrayHome:
                    return _myColor != Colors.Gray;
                case Items.WhiteHome:
                    return _myColor != Colors.White;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Take in an action and return its opposite direction.
        /// </summary>
        /// <param name="a">Direction to get the opposite.</param>
        /// <returns>Opposite direction of action, Wait if no opposite exists.</returns>
        private static Actions OppositeDirection(Actions a)
        {
            switch (a)
            {
                case Actions.MoveRight:
                    return Actions.MoveLeft;
                case Actions.MoveDown:
                    return Actions.MoveUp;
                case Actions.MoveLeft:
                    return Actions.MoveRight;
                case Actions.MoveUp:
                    return Actions.MoveDown;
                case Actions.EchoRight:
                    return Actions.EchoLeft;
                case Actions.EchoDown:
                    return Actions.EchoUp;
                case Actions.EchoLeft:
                    return Actions.EchoRight;
                case Actions.EchoUp:
                    return Actions.EchoDown;
                case Actions.ShootRight:
                    return Actions.ShootLeft;
                case Actions.ShootDown:
                    return Actions.ShootUp;
                case Actions.ShootLeft:
                    return Actions.ShootRight;
                case Actions.ShootUp:
                    return Actions.ShootDown;
                default:
                    return Actions.Wait;
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
            public Items Item { get; set; }
        }
    }
}
