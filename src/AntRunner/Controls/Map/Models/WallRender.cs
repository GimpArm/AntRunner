using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using AntRunner.Interface;
using AntRunner.Models;

namespace AntRunner.Controls.Map.Models
{
    public class WallRender
    {
        private static readonly Size ArcSize = new Size(6,6);
        private static readonly Size CornerSize = new Size(4, 4);

        public enum TileSide
        {
            Left,
            Top,
            Right,
            Bottom
        }
        
        private readonly AntRunner.Models.Map _map;
        private readonly Item _wallType;
        private readonly Path _path;
        private readonly CombinedGeometry _geometry;

        private readonly IList<MapTile> _tiles = new List<MapTile>();

        public Panel Parent => _path?.Parent as Panel;

        public WallRender(AntRunner.Models.Map map, Item wallType)
        {
            _map = map;
            _wallType = wallType;
            if (wallType == Item.SteelWall)
            {

                _path = new Path
                {
                    StrokeThickness = 1,
                    Fill = Brushes.DimGray,
                    Stroke = Brushes.Black
                };
            }
            else
            {
                _path = new Path
                {
                    StrokeThickness = 1,
                    Fill = Brushes.Brown,
                    Stroke = Brushes.Black
                };
            }

            _geometry = new CombinedGeometry{GeometryCombineMode = GeometryCombineMode.Exclude, Geometry2 = new GeometryGroup()};
            _path.Data = _geometry;
        }

        public void Redraw(MapTile tile)
        {
            var panel = Parent;
            Parent.Children.Remove(_path);
            tile.Processed = null;
            _tiles.Remove(tile);
            var allTiles = _tiles.OrderBy(x => x.Y).ThenBy(x => x.X).Select(x =>
            {
                x.Processed = null;
                return x;
            }).ToList();
            _tiles.Clear();

            var current = allTiles.FirstOrDefault(x => x.Processed == null);
            while (current != null)
            {
                if (Create(_map, current))
                {
                    var newWall = new WallRender(_map, _wallType);
                    panel.Children.Insert(0, newWall.Render(current));
                }
                current = allTiles.FirstOrDefault(x => x.Processed == null);
            }
        }

        public void Combine(MapTile tile, TileSide direction)
        {
            AddTile(tile);
            var pathFigure = FigureRender(tile, direction, true);
            ((GeometryGroup)_geometry.Geometry2).Children.Add(new PathGeometry(new[] { pathFigure }));
        }

        public Path Render(MapTile tile, TileSide direction = TileSide.Left)
        {
            _tiles.Clear();
            AddTile(tile);
            var pathFigure = FigureRender(tile, direction);
            _geometry.Geometry1 = new PathGeometry(new[] { pathFigure });
            return _path;
        }

        private PathFigure FigureRender(MapTile tile, TileSide direction = TileSide.Left, bool inside = false)
        {
            var pathFigure = new PathFigure();
            AddTile(tile);

            var position = CalcPosition(direction, tile);
            pathFigure.StartPoint = position;
            var insidePath = new Dictionary<Point, KeyValuePair<TileSide, MapTile>>();
            do
            {
                if (!inside)
                {
                    var opposite = Opposite(direction);
                    if (GetTile(tile, opposite) is MapTile opTile && opTile.Item != _wallType)
                    {
                        var checkPoint = CalcPosition(opposite, tile);
                        if (!insidePath.ContainsKey(checkPoint))
                        {
                            insidePath.Add(checkPoint, new KeyValuePair<TileSide, MapTile>(opposite, tile));
                        }
                    }
                }

                switch (direction)
                {
                    case TileSide.Left:
                        if (UpLeft(tile) is MapTile upLeft)
                        {
                            tile = upLeft;
                            direction = TileSide.Bottom;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new ArcSegment { Point = position, Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                        }
                        else if (Up(tile) is MapTile up)
                        {
                            tile = up;
                            direction = TileSide.Left;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new LineSegment { Point = position });
                        }
                        else if (tile.X == 0)
                        {
                            if (Up(tile, true) is MapTile upEdge)
                            {
                                position = CalcPosition(TileSide.Left, upEdge);
                                pathFigure.Segments.Add(new LineSegment { Point = position });
                                direction = TileSide.Top;
                                position = CalcPosition(direction, tile);
                                pathFigure.Segments.Add(new ArcSegment { Point = position, Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                            }
                            else
                            {
                                pathFigure.Segments.Add(new LineSegment { Point = new Point(0, 0) });
                                direction = TileSide.Top;
                                position = CalcPosition(direction, tile);
                                pathFigure.Segments.Add(new LineSegment { Point = position });
                            }
                        }
                        else if (tile.Y == 0)
                        {
                            pathFigure.Segments.Add(new ArcSegment { Point = CalcPosition(TileSide.Top, Left(tile, true)), Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                            direction = TileSide.Top;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new LineSegment { Point = position });
                        }
                        else
                        {
                            direction = TileSide.Top;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new ArcSegment { Point = position, Size = CornerSize, SweepDirection = SweepDirection.Clockwise });
                        }
                        break;
                    case TileSide.Top:
                        if (UpRight(tile) is MapTile upRight)
                        {
                            tile = upRight;
                            direction = TileSide.Left;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new ArcSegment { Point = position, Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                        }
                        else if (Right(tile) is MapTile right)
                        {
                            tile = right;
                            direction = TileSide.Top;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new LineSegment { Point = position });
                        }
                        else if (tile.X == _map.MaxWidth)
                        {
                            if (Up(tile, true) is MapTile upEdge)
                            {
                                pathFigure.Segments.Add(new ArcSegment { Point = CalcPosition(TileSide.Right, upEdge), Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                                direction = TileSide.Right;
                                position = CalcPosition(direction, tile);
                                pathFigure.Segments.Add(new LineSegment { Point = position });
                            }
                            else
                            {
                                pathFigure.Segments.Add(new LineSegment { Point = new Point(_map.Width * 10, 0) });
                                direction = TileSide.Right;
                                position = CalcPosition(direction, tile);
                                pathFigure.Segments.Add(new LineSegment { Point = position });
                            }
                        }
                        else if (tile.Y == 0)
                        {
                            pathFigure.Segments.Add(new LineSegment { Point = CalcPosition(TileSide.Top, Right(tile, true)) });
                            direction = TileSide.Right;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new ArcSegment { Point = position, Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                        }
                        else
                        {

                            direction = TileSide.Right;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new ArcSegment { Point = position, Size = CornerSize, SweepDirection = SweepDirection.Clockwise });
                        }
                        break;
                    case TileSide.Right:
                        if (DownRight(tile) is MapTile downRight)
                        {
                            tile = downRight;
                            direction = TileSide.Top;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new ArcSegment { Point = position, Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                        }
                        else if (Down(tile) is MapTile down)
                        {
                            tile = down;
                            direction = TileSide.Right;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new LineSegment { Point = position });
                        }
                        else if (tile.X == _map.MaxWidth)
                        {
                            if (Down(tile, true) is MapTile downEdge)
                            {
                                pathFigure.Segments.Add(new LineSegment { Point = CalcPosition(TileSide.Right, downEdge) });
                                direction = TileSide.Bottom;
                                position = CalcPosition(direction, tile);
                                pathFigure.Segments.Add(new ArcSegment { Point = position, Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                            }
                            else
                            {
                                pathFigure.Segments.Add(new LineSegment { Point = new Point(_map.Width * 10, _map.Height * 10) });
                                direction = TileSide.Bottom;
                                position = CalcPosition(direction, tile);
                                pathFigure.Segments.Add(new LineSegment { Point = position });
                            }
                        }
                        else if (tile.Y == _map.MaxHeight)
                        {
                            pathFigure.Segments.Add(new ArcSegment { Point = CalcPosition(TileSide.Bottom, Right(tile, true)), Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                            direction = TileSide.Bottom;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new LineSegment { Point = position });
                        }
                        else
                        {
                            direction = TileSide.Bottom;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new ArcSegment { Point = position, Size = CornerSize, SweepDirection = SweepDirection.Clockwise });
                        }
                        break;
                    case TileSide.Bottom:
                        if (DownLeft(tile) is MapTile downLeft)
                        {
                            tile = downLeft;
                            direction = TileSide.Right;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new ArcSegment { Point = position, Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                        }
                        else if (Left(tile) is MapTile left)
                        {
                            tile = left;
                            direction = TileSide.Bottom;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new LineSegment { Point = position });
                        }
                        else if (tile.X == 0)
                        {
                            if (Down(tile, true) is MapTile downEdge)
                            {
                                pathFigure.Segments.Add(new ArcSegment { Point = CalcPosition(TileSide.Left, downEdge), Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                                direction = TileSide.Left;
                                position = CalcPosition(direction, tile);
                                pathFigure.Segments.Add(new LineSegment { Point = position });
                            }
                            if (tile.Y == _map.MaxHeight)
                            {
                                pathFigure.Segments.Add(new LineSegment { Point = new Point(0, _map.Height * 10) });
                                direction = TileSide.Left;
                                position = CalcPosition(direction, tile);
                                pathFigure.Segments.Add(new LineSegment { Point = position });
                            }
                        }
                        else if (tile.Y == _map.MaxHeight)
                        {
                            pathFigure.Segments.Add(new LineSegment { Point = CalcPosition(TileSide.Bottom, Left(tile, true)) });
                            direction = TileSide.Left;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new ArcSegment { Point = position, Size = ArcSize, SweepDirection = SweepDirection.Counterclockwise });
                        }
                        else
                        {
                            direction = TileSide.Left;
                            position = CalcPosition(direction, tile);
                            pathFigure.Segments.Add(new ArcSegment { Point = position, Size = CornerSize, SweepDirection = SweepDirection.Clockwise });
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                AddTile(tile);
                if (insidePath.ContainsKey(position))
                {
                    insidePath.Remove(position);
                }
            } while (pathFigure.StartPoint != position);

            if (!inside && insidePath.Any())
            {
                foreach (var segment in pathFigure.Segments)
                {
                    if (segment is ArcSegment arc)
                    {
                        if (insidePath.ContainsKey(arc.Point))
                        {
                            insidePath.Remove(arc.Point);
                        }
                    }
                    else if (segment is LineSegment line)
                    {
                        if (insidePath.ContainsKey(line.Point))
                        {
                            insidePath.Remove(line.Point);
                        }
                    }
                }
                if (insidePath.Any())
                {
                    var insideWall = insidePath.Values.First();
                    Combine(insideWall.Value, insideWall.Key);
                }
            }

            return pathFigure;
        }

        private MapTile GetTile(MapTile tile, TileSide direction)
        {
            switch (direction)
            {
                case TileSide.Left:
                    return Left(tile, true);
                case TileSide.Top:
                    return Up(tile, true);
                case TileSide.Right:
                    return Right(tile, true);
                case TileSide.Bottom:
                    return Down(tile, true);
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }

        private MapTile Left(MapTile tile, bool ignoreType = false)
        {
            if (tile.X <= 0) return null;
            var result = _map[tile.X - 1, tile.Y];
            return ignoreType || result.Item == _wallType ? result : null;
        }

        private MapTile Right(MapTile tile, bool ignoreType = false)
        {
            if (tile.X >= _map.MaxWidth) return null;
            var result = _map[tile.X + 1, tile.Y];
            return ignoreType || result.Item == _wallType ? result : null;
        }

        private MapTile Down(MapTile tile, bool ignoreType = false)
        {
            if (tile.Y >= _map.MaxHeight) return null;
            var result = _map[tile.X, tile.Y + 1];
            return ignoreType || result.Item == _wallType ? result : null;
        }

        private MapTile Up(MapTile tile, bool ignoreType = false)
        {
            if (tile.Y > 0)
            {
                var result = _map[tile.X, tile.Y - 1];
                return ignoreType || result.Item == _wallType ? result : null;
            }
            return null;
        }
        
        private MapTile UpLeft(MapTile tile, bool ignoreType = false)
        {
            if (tile.X <= 0 || tile.Y <= 0) return null;
            var result = _map[tile.X - 1, tile.Y - 1];
            return ignoreType || result.Item == _wallType ? result : null;
        }

        private MapTile UpRight(MapTile tile, bool ignoreType = false)
        {
            if (tile.X >= _map.MaxWidth || tile.Y <= 0) return null;
            var result = _map[tile.X + 1, tile.Y - 1];
            return ignoreType || result.Item == _wallType ? result : null;
        }

        private MapTile DownLeft(MapTile tile, bool ignoreType = false)
        {
            if (tile.X <= 0 || tile.Y >= _map.MaxHeight) return null;
            var result = _map[tile.X - 1, tile.Y + 1];
            return ignoreType || result.Item == _wallType ? result : null;
        }

        private MapTile DownRight(MapTile tile, bool ignoreType = false)
        {
            if (tile.X >= _map.MaxWidth || tile.Y >= _map.MaxHeight) return null;
            var result = _map[tile.X + 1, tile.Y + 1];
            return ignoreType || result.Item == _wallType ? result : null;
        }

        public static bool Create(AntRunner.Models.Map map, MapTile tile)
        {
            var checker = new WallRender(map, tile.Item);
            var sides = new[]
            {
                new KeyValuePair<TileSide, MapTile>(TileSide.Left, checker.Left(tile, true)), new KeyValuePair<TileSide, MapTile>(TileSide.Right, checker.Right(tile, true)),
                new KeyValuePair<TileSide, MapTile>(TileSide.Bottom, checker.Down(tile, true)), new KeyValuePair<TileSide, MapTile>(TileSide.Top, checker.Up(tile, true))
            }.Where(x => x.Value != null).ToArray();

            if (sides.All(x => x.Value.Item == tile.Item))
            {
                var first = sides.Select(x => x.Value).FirstOrDefault(x => x.Processed != null);
                if (first != null)
                {
                    first.Processed.AddTile(tile);
                    return false;
                }
                return true;
            }

            var group = sides.FirstOrDefault(x => x.Value.Item == tile.Item && x.Value.Processed != null);
            if (group.Value == null) return true;

            var startTile = sides.FirstOrDefault(x => x.Value.Item != tile.Item);
            group.Value.Processed.Combine(tile, startTile.Key);
            return false;
        }

        private void AddTile(MapTile tile)
        {
            if (!_tiles.Contains(tile))
            {
                _tiles.Add(tile);
            }

            tile.Processed = this;
        }

        private static Point CalcPosition(TileSide direction, MapTile tile)
        {
            switch (direction)
            {
                case TileSide.Left:
                    return new Point(tile.X * 10, tile.Y * 10 + 5);
                case TileSide.Top:
                    return new Point(tile.X * 10 + 5, tile.Y * 10);
                case TileSide.Right:
                    return new Point((tile.X + 1) * 10, tile.Y * 10 + 5);
                case TileSide.Bottom:
                    return new Point(tile.X * 10 + 5, (tile.Y + 1) * 10);
                default:
                    throw new Exception("Invalid TileSite");
            }
        }

        private static TileSide Opposite(TileSide direction)
        {
            switch (direction)
            {
                case TileSide.Left:
                    return TileSide.Right;
                case TileSide.Top:
                    return TileSide.Bottom;
                case TileSide.Right:
                    return TileSide.Left;
                case TileSide.Bottom:
                    return TileSide.Top;
                default:
                    throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
            }
        }
    }
}
