using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using AntRunner.Enums;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public class Map : IEnumerable<MapTile>
    {
        private const int SteelWallColor = -16777216; //black
        private const int BrickWallColor = -65536; //red
        private const int FlagColor = -16711936; //green
        private const int HomeColor = -16776961; // blue

        private readonly MapTile[,] _tiles;
        public int Width { get; }
        public int Height { get; }

        public int MaxWidth { get; }
        public int MaxHeight { get; }

        public Map(IList<ItemColor> colorList, MapSize mapSize)
        {
            var map = new MapGenerator(mapSize);
            _tiles = map.Generate();
            Width = map.Width;
            Height = map.Height;
            MaxWidth = Width - 1;
            MaxHeight = Height - 1;

            Initialize(colorList);
        }

        public Map(Bitmap mapDefinition, IList<ItemColor> colorList)
        {
            Width = mapDefinition.Width;
            Height = mapDefinition.Height;
            MaxWidth = Width - 1;
            MaxHeight = Height - 1;
            _tiles = new MapTile[Width, Height];

            var possibleFlags = new List<MapTile>();
            var possibleHomes = new List<MapTile>();


            for (var y = 0; y < Height; ++y)
            {
                for (var x = 0; x < Width; ++x)
                {
                    var tile = new MapTile(x, y);
                    _tiles[x, y] = tile;
                    var pixel = mapDefinition.GetPixel(x, y).ToArgb();

                    switch (pixel)
                    {
                        case SteelWallColor:
                            tile.Item = Item.SteelWall;
                            break;
                        case BrickWallColor:
                            tile.Item = Item.BrickWall;
                            break;
                        case FlagColor:
                            possibleFlags.Add(tile);
                            break;
                        case HomeColor:
                            possibleHomes.Add(tile);
                            break;
                    }
                }
            }

            Initialize(colorList, possibleFlags, possibleHomes);
        }

        private void Initialize(IList<ItemColor> colorList, List<MapTile> possibleFlags = null, List<MapTile> possibleHomes = null)
        {
            if (possibleFlags != null && possibleFlags.Any())
            {
                possibleFlags.Shuffle();
                possibleFlags[Utilities.Random.Next(0, possibleFlags.Count - 1)].Item = Item.Flag;
            }
            else
            {
                RandomTile().Item = Item.Flag;
            }

            colorList.Shuffle();
            if (possibleHomes == null)
            {
                possibleHomes = new List<MapTile>();
            }
            else
            {
                possibleHomes.Shuffle();
            }

            while (colorList.Any())
            {
                var color = colorList[Utilities.Random.Next(0, colorList.Count - 1)];
                MapTile tile;
                if (possibleHomes.Any())
                {
                    tile = possibleHomes[Utilities.Random.Next(0, possibleHomes.Count - 1)];
                    possibleHomes.Remove(tile);
                }
                else
                {
                    tile = RandomTile();
                }
                tile.Item = Utilities.ColorToAntHomeItem(color);
                colorList.Remove(color);
            }
        }

        public MapTile RandomTile()
        {
            MapTile tile;
            do
            {
                tile = _tiles[Utilities.Random.Next(0, Width - 1), Utilities.Random.Next(0, Height - 1)];
            } while (tile.Item != Item.Empty);

            return tile;
        }

        public MapTile this[int x, int y] => _tiles[x, y];

        public GameEvent MoveTo(int x, int y, AntWrapper ant, Action<AntWrapper, GameEvent> sideEvents)
        {
            if (IsEdge(x, y)) return GameEvent.CollisionDamage;

            var tile = this[x, y];

            var eventResult = GameEvent.Nothing;
            switch (tile.Item)
            {
                case Item.Empty:
                    eventResult = GameEvent.Nothing;
                    break;
                case Item.SteelWall:
                case Item.BrickWall:
                    return GameEvent.CollisionDamage;
                case Item.Bomb:
                    eventResult = GameEvent.BombDamage;
                    break;
                case Item.PowerUpBomb:
                    eventResult = GameEvent.PickUpBomb;
                    break;
                case Item.PowerUpHealth:
                    eventResult = GameEvent.PickUpHealth;
                    break;
                case Item.PowerUpShield:
                    eventResult = GameEvent.PickUpShield;
                    break;
                case Item.Flag:
                    eventResult = GameEvent.PickUpFlag;
                    break;
                case Item.RedHome:
                case Item.BlueHome:
                case Item.GreenHome:
                case Item.OrangeHome:
                case Item.PinkHome:
                case Item.YellowHome:
                case Item.GrayHome:
                case Item.WhiteHome:
                    if (ant.HasFlag && tile.Item == ant.AntHome)
                    {
                        return GameEvent.GameOver;
                    }
                    return GameEvent.CollisionDamage;
            }

            if (tile.OccupiedBy != null)
            {
                AntCollision(x, y, ant, tile, sideEvents);
                return GameEvent.CollisionDamage;
            }

            ant.CurrentTile = tile;
            tile.Item = Item.Empty;
            return eventResult;
        }

        private static void AntCollision(int x, int y, AntWrapper ant, MapTile tile, Action<AntWrapper, GameEvent> sideEvents)
        {
            if (x == ant.CurrentTile.X)
            {
                sideEvents(tile.OccupiedBy, y < ant.CurrentTile.Y ? GameEvent.ImpactDamageDown : GameEvent.ImpactDamageUp);
            }
            else
            {
                sideEvents(tile.OccupiedBy, x < ant.CurrentTile.X ? GameEvent.ImpactDamageRight : GameEvent.ImpactDamageLeft);
            }
        }

        public MapTile GetTileTo(AntWrapper ant, AntAction action)
        {
            var x = ant.CurrentTile.X;
            var y = ant.CurrentTile.Y;
            switch (action)
            {
                case AntAction.ShootRight:
                case AntAction.EchoRight:
                    return GetTileTo(() => ++x, () => y);
                case AntAction.ShootDown:
                case AntAction.EchoDown:
                    return GetTileTo(() => x, () => ++y);
                case AntAction.ShootLeft:
                case AntAction.EchoLeft:
                    return GetTileTo(() => --x, () => y);
                case AntAction.ShootUp:
                case AntAction.EchoUp:
                    return GetTileTo(() => x, () => --y);
            }

            return null;
        }

        private MapTile GetTileTo(Func<int> getX, Func<int> getY)
        {
            MapTile tile;
            do
            {
                var x = getX();
                var y = getY();
                if (IsEdge(x, y))
                {
                    return OuterWall(x, y);
                }
                tile = this[x, y];
            } while (tile.Item == Item.Empty && tile.OccupiedBy == null);

            return tile;
        }

        private static MapTile OuterWall(int x, int y)
        {
            return new MapTile(x, y){Item = Item.SteelWall};
        }

        private bool IsEdge(int x, int y)
        {
            return x >= Width || y >= Height || x < 0 || y < 0;
        }

        public IEnumerator<MapTile> GetEnumerator()
        {
            return _tiles.Cast<MapTile>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
