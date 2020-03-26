using AntRunner.Enums;

namespace AntRunner.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Interface;

    public class MapGenerator
    {
        private readonly Random _random;
        private readonly int _width;
        private readonly int _height;
        private readonly int _squareSize;
        private readonly int _brickPercentage;

        private readonly int _xLimit;
        private readonly int _yLimit;

        private readonly Walls[,] _mapArray;

        public int Width => _width * _squareSize;
        public int Height => _height * _squareSize;

        public MapGenerator(MapSize mapSize)
        {
            _random = new Random(DateTime.Now.Millisecond);
            if (mapSize == MapSize.Random)
            {
                mapSize = (MapSize)_random.Next(0, 2);
            }
            switch (mapSize)
            {
                case MapSize.Small:
                    _width = _random.Next(5, 15);
                    _height = _random.Next(5, 10);
                    _squareSize = 4;
                    break;
                case MapSize.Medium:
                    _width = _random.Next(10, 25);
                    _height = _random.Next(10, 20);
                    _squareSize = _random.Next(4, 5);
                    break;
                case MapSize.Large:
                    _width = _random.Next(25, 50);
                    _height = _random.Next(20, 50);
                    _squareSize = _random.Next(4, 7);
                    break;
            }

            _brickPercentage = _random.Next(5, 25);
            _xLimit = _width - 1;
            _yLimit = _height - 1;
            _mapArray = new Walls[_width, _height];
        }

        public MapGenerator(int width = 0, int height = 0, int squareSize = 0)
        {
            _random = new Random(DateTime.Now.Millisecond);
            _width = width == 0 ? _random.Next(20, 50) : width;
            _height = height == 0 ? _random.Next(15, 30) : height;
            _squareSize = squareSize < 3 ? _random.Next(4, 7) : squareSize;

            _xLimit = _width - 1;
            _yLimit = _height - 1;
            _mapArray = new Walls[_width, _height];

        }

        public MapTile[,] Generate()
        {
            RandomWalls();
            OpenBoxes();
            EnsureConnections();
            var map = GenerateMap();
            InsertRandomBricks(ref map);
            return map;
        }

        private void RandomWalls()
        {
            for (var y = 0; y < _height; ++y)
            {
                for (var x = 0; x < _width; ++ x)
                {
                    if (_random.Next(0, 100) >= 50)
                    {
                        _mapArray[x, y] |= Walls.Right;
                        if (x < _xLimit)
                        {
                            _mapArray[x + 1, y] |= Walls.Left;
                        }
                    }
                    if (_random.Next(0, 100) >= 50)
                    {
                        _mapArray[x, y] |= Walls.Bottom;
                        if (y < _yLimit)
                        {
                            _mapArray[x, y + 1] |= Walls.Top;
                        }
                    }
                }
            }
        }

        private void OpenBoxes()
        {
            for (var y = 0; y < _height; ++y)
            {
                for (var x = 0; x < _width; ++x)
                {
                    while (_mapArray[x, y].HasFlag(Walls.All)
                        || (y == 0 && _mapArray[x, y].HasFlag(Walls.Bottom | Walls.Left | Walls.Right))
                        || (x == 0 && _mapArray[x, y].HasFlag(Walls.Bottom | Walls.Top | Walls.Right))
                        || (y == _yLimit && _mapArray[x, y].HasFlag(Walls.Top | Walls.Left | Walls.Right))
                        || (x == _xLimit && _mapArray[x, y].HasFlag(Walls.Bottom | Walls.Left | Walls.Top)))
                    {
                        var del = (Walls) _random.Next(1, 15);
                        if (del.HasFlag(Walls.Left))
                        {
                            _mapArray[x, y] &= ~Walls.Left;
                            if (x > 0)
                            {
                                _mapArray[x - 1, y] &= ~Walls.Right;
                            }
                        }

                        if (del.HasFlag(Walls.Right))
                        {
                            _mapArray[x, y] &= ~Walls.Right;
                            if (x < _xLimit)
                            {
                                _mapArray[x + 1, y] &= ~Walls.Left;
                            }
                        }

                        if (del.HasFlag(Walls.Top))
                        {
                            _mapArray[x, y] &= ~Walls.Top;
                            if (y > 0)
                            {
                                _mapArray[x, y - 1] &= ~Walls.Bottom;
                            }
                        }

                        if (del.HasFlag(Walls.Bottom))
                        {
                            _mapArray[x, y] &= ~Walls.Bottom;
                            if (y < _yLimit)
                            {
                                _mapArray[x, y + 1] &= ~Walls.Top;
                            }
                        }
                    }
                }
            }
        }

        private void EnsureConnections()
        {
            var visit = new bool[_width, _height];
            visit[0, 0] = true;
            var posX = 0;
            var posY = 0;
            var junction = new Stack<Tuple<int, int>>();
            var deadEnd = new Queue<Tuple<int, int>>();
            while (visit.Cast<bool>().Any(x => !x))
            {
                var spot = new Tuple<int, int>(posX, posY);
                Tuple<int, int> next = null;
                var wayCnt = 0;
                if (posX < _xLimit && !_mapArray[posX, posY].HasFlag(Walls.Right) && !visit[posX + 1, posY])
                {
                    wayCnt++;
                    next = new Tuple<int, int>(posX + 1, posY);
                }
                if (posY < _yLimit && !_mapArray[posX, posY].HasFlag(Walls.Bottom) && !visit[posX, posY + 1])
                {
                    wayCnt++;
                    next = new Tuple<int, int>(posX, posY + 1);
                }
                if (posX > 0 && !_mapArray[posX, posY].HasFlag(Walls.Left) && !visit[posX - 1, posY])
                {
                    wayCnt++;
                    next = new Tuple<int, int>(posX - 1, posY);
                }
                if (posY > 0 && !_mapArray[posX, posY].HasFlag(Walls.Top) && !visit[posX, posY - 1])
                {
                    wayCnt++;
                    next = new Tuple<int, int>(posX, posY - 1);
                }

                while (wayCnt > 1)
                {
                    junction.Push(spot);
                    wayCnt--;
                }
                if (next == null)
                {
                    var found = false;
                    if (junction.Any())
                    {
                        deadEnd.Enqueue(spot);
                        var lastJunk = junction.Pop();
                        posX = lastJunk.Item1;
                        posY = lastJunk.Item2;
                        found = true;
                    }
                    else if (deadEnd.Any())
                    {
                        found = true;
                        var lastEnd = deadEnd.Dequeue();
                        while (!AnyOpen(visit, lastEnd.Item1, lastEnd.Item2))
                        {
                            if (!deadEnd.Any())
                            {
                                found = false;
                                break;
                            }
                            lastEnd = deadEnd.Dequeue();
                        }
                    }

                    if (found) continue;

                    int x1 = _xLimit, y1 = _yLimit;

                    for (; x1 >= 0; --x1)
                    {
                        for (y1 = _yLimit; y1 >= 0; --y1)
                        {
                            if (!visit[x1, y1] && AnyOpen(visit, x1, y1, true))
                            {
                                found = true;
                                break;
                            }
                        }

                        if (found)
                        {
                            break;
                        }
                    }

                    next = new Tuple<int, int>(x1, y1);
                }

                posX = next.Item1;
                posY = next.Item2;
                visit[posX, posY] = true;
            }
        }

        private bool AnyOpen(bool[,] visited, int x, int y, bool inverseVisit = false)
        {
            foreach (var c in new[] {1, 2, 3, 4}.OrderBy(i => _random.Next()))
            {
                switch (c)
                {
                    case 1:
                        if (x > 0 && (!inverseVisit && !visited[x - 1, y] || inverseVisit && visited[x - 1, y]))
                        {
                            _mapArray[x, y] &= ~Walls.Left;
                            _mapArray[x - 1, y] &= ~Walls.Right;
                            return true;
                        }
                        break;
                    case 2:
                        if (x < _xLimit && (!inverseVisit && !visited[x + 1, y] || inverseVisit && visited[x + 1, y]))
                        {
                            _mapArray[x, y] &= ~Walls.Right;
                            _mapArray[x + 1, y] &= ~Walls.Left;
                            return true;
                        }
                        break;
                    case 3:
                        if (y > 0 && (!inverseVisit && !visited[x, y - 1] || inverseVisit && visited[x, y - 1]))
                        {
                            _mapArray[x, y] &= ~Walls.Top;
                            _mapArray[x, y - 1] &= ~Walls.Bottom;
                            return true;
                        }
                        break;
                    case 4:
                        if (y < _yLimit && (!inverseVisit && !visited[x, y + 1] || inverseVisit && visited[x, y + 1]))
                        {
                            _mapArray[x, y] &= ~Walls.Bottom;
                            _mapArray[x, y + 1] &= ~Walls.Top;
                            return true;
                        }
                        break;
                }
            }

            return false;
        }

        private MapTile[,] GenerateMap()
        {
            var map = new MapTile[Width, Height];

            for (var y = 0; y < _height * _squareSize; ++y)
            {
                for (var x = 0; x < _width * _squareSize; ++x)
                {
                    map[x, y] = new MapTile(x, y);
                }
            }

            for (var y = 0; y < _height; ++y)
            {
                for (var x = 0; x < _width; ++x)
                {
                    if (_mapArray[x, y].HasFlag(Walls.Left))
                    {
                        DrawSquares(ref map, Item.SteelWall, (x * _squareSize), y * _squareSize, 1, _squareSize + 1);
                    }
                    if (_mapArray[x, y].HasFlag(Walls.Top))
                    {
                        DrawSquares(ref map, Item.SteelWall, x * _squareSize, (y * _squareSize), _squareSize + 1, 1);
                    }
                }
            }

            return map;
        }

        private void DrawSquares(ref MapTile[,] map, Item wallType, int x, int y, int width, int height)
        {
            var xLimit = x + width;
            if (xLimit > Width)
            {
                xLimit = Width;
            }
            var yLimit = y + height;
            if (yLimit > Height)
            {
                yLimit = Height;
            }

            for (; y < yLimit; ++y)
            {
                for (var x1 = x; x1 < xLimit; ++x1)
                {
                    map[x1, y].Item = wallType;
                }
            }
        }

        private void InsertRandomBricks(ref MapTile[,] map)
        {
            var brickSquares = (int) (_width * _height * _brickPercentage / 100);
            for (var i = 0; i < brickSquares; ++i)
            {
                var x = _random.Next(0, _xLimit);
                var y = _random.Next(0, _yLimit);
                if (_random.Next(0, 100) >= 25)
                {
                    var brick = (Walls) _random.Next(1, 15);
                    if (brick.HasFlag(Walls.Left) && !_mapArray[x, y].HasFlag(Walls.Left))
                    {
                        var length = _squareSize - (_mapArray[x, y].HasFlag(Walls.Top) ? 1 : 0);
                        var offset = _mapArray[x, y].HasFlag(Walls.Top) ? 1 : 0;
                        DrawSquares(ref map, Item.BrickWall, x * _squareSize, (y * _squareSize) + offset, 1, length);
                    }

                    if (brick.HasFlag(Walls.Right) && !_mapArray[x, y].HasFlag(Walls.Right))
                    {
                        var length = _squareSize - (_mapArray[x, y].HasFlag(Walls.Top) ? 1 : 0) + (_mapArray[x, y].HasFlag(Walls.Bottom) ? 0 : 1);
                        var offset = _mapArray[x, y].HasFlag(Walls.Top) ? 1 : 0;
                        DrawSquares(ref map, Item.BrickWall, (x + 1) * _squareSize, (y * _squareSize) + offset, 1, length);
                    }

                    if (brick.HasFlag(Walls.Top) && !_mapArray[x, y].HasFlag(Walls.Top))
                    {
                        var length = _squareSize - (_mapArray[x, y].HasFlag(Walls.Left) ? 1 : 0);
                        var offset = _mapArray[x, y].HasFlag(Walls.Left) ? 1 : 0;
                        DrawSquares(ref map, Item.BrickWall, (x * _squareSize) + offset, y * _squareSize, length, 1);
                    }

                    if (brick.HasFlag(Walls.Bottom) && !_mapArray[x, y].HasFlag(Walls.Bottom))
                    {
                        var length = _squareSize - (_mapArray[x, y].HasFlag(Walls.Left) ? 1 : 0) + (_mapArray[x, y].HasFlag(Walls.Right) ? 0 : 1);
                        var offset = _mapArray[x, y].HasFlag(Walls.Left) ? 1 : 0;
                        DrawSquares(ref map, Item.BrickWall, (x * _squareSize) + offset, (y + 1) * _squareSize, length, 1);
                    }
                }
                else
                {
                    var length = _squareSize - (_mapArray[x, y].HasFlag(Walls.Top) ? 1 : 0) + (_mapArray[x, y].HasFlag(Walls.Bottom) ? 0 : 1);
                    var width = _squareSize - (_mapArray[x, y].HasFlag(Walls.Left) ? 1 : 0) + (_mapArray[x, y].HasFlag(Walls.Right) ? 0 : 1);

                    var xOffset = _mapArray[x, y].HasFlag(Walls.Left) ? 1 : 0;
                    var yOffset = _mapArray[x, y].HasFlag(Walls.Top) ? 1 : 0;

                    if (_random.Next(0, 100) >= 75)
                    {
                        DrawSquares(ref map, Item.BrickWall, (x * _squareSize) + xOffset, (y * _squareSize) + yOffset, width, length);
                    }
                    else
                    {
                        var middle = (int) Math.Ceiling(_squareSize / 2m);
                        DrawSquares(ref map, Item.BrickWall, (x * _squareSize) + middle, (y * _squareSize) + yOffset, 1, length);
                        DrawSquares(ref map, Item.BrickWall, (x * _squareSize) + xOffset, (y * _squareSize) + middle, width, 1);
                    }
                }
            }
        }

        [Flags]
        private enum Walls
        {
            None = 0x00,
            Left = 0x01,
            Right = 0x02,
            Top = 0x04,
            Bottom = 0x08,
            All = 0x0F
        }
    }
}
