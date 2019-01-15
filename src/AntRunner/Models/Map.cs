using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
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

        public Map(Bitmap mapDefinition, IList<Colors> colorList)
        {
            Width = mapDefinition.Width;
            Height = mapDefinition.Height;
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
                            tile.Item = Items.SteelWall;
                            break;
                        case BrickWallColor:
                            tile.Item = Items.BrickWall;
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

            if (possibleFlags.Any())
            {
                possibleFlags[Utilities.Random.Next(0, possibleFlags.Count - 1)].Item = Items.Flag;
            }
            else
            {
                RandomTile().Item = Items.Flag;
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
            } while (tile.Item != Items.Nothing);

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
                case Items.Nothing:
                    eventResult = GameEvent.Nothing;
                    break;
                case Items.SteelWall:
                case Items.BrickWall:
                    return GameEvent.CollisionDamage;
                case Items.Bomb:
                    eventResult = GameEvent.BombDamage;
                    break;
                case Items.PowerUpBomb:
                    eventResult = GameEvent.PickUpBomb;
                    break;
                case Items.PowerUpHealth:
                    eventResult = GameEvent.PickUpHealth;
                    break;
                case Items.PowerUpShield:
                    eventResult = GameEvent.PickUpShield;
                    break;
                case Items.Flag:
                    eventResult = GameEvent.PickUpFlag;
                    break;
                case Items.RedAnt:
                case Items.BlueAnt:
                case Items.GreenAnt:
                case Items.OrangeAnt:
                case Items.PinkAnt:
                case Items.YellowAnt:
                case Items.GrayAnt:
                case Items.WhiteAnt:
                    AntCollision(x, y, ant, tile, sideEvents);
                    return GameEvent.CollisionDamage;
                case Items.RedHome:
                case Items.BlueHome:
                case Items.GreenHome:
                case Items.OrangeHome:
                case Items.PinkHome:
                case Items.YellowHome:
                case Items.GrayHome:
                case Items.WhiteHome:
                    if (ant.HasFlag && tile.Item == ant.AntHome)
                    {
                        return GameEvent.GameOver;
                    }
                    return GameEvent.CollisionDamage;
            }
            
            ant.CurrentTile = tile;
            return eventResult;
        }

        private static void AntCollision(int x, int y, AntWrapper ant, MapTile tile, Action<AntWrapper, GameEvent> sideEvents)
        {
            if (tile.OccupiedBy == null) return;
            if (x == ant.CurrentTile.X)
            {
                sideEvents(tile.OccupiedBy, y < ant.CurrentTile.Y ? GameEvent.ImpactDamageDown : GameEvent.ImpactDamageUp);
            }
            else
            {
                sideEvents(tile.OccupiedBy, x < ant.CurrentTile.X ? GameEvent.ImpactDamageRight : GameEvent.ImpactDamageLeft);
            }
        }

        public MapTile GetTileTo(AntWrapper ant, Actions action)
        {
            var x = ant.CurrentTile.X;
            var y = ant.CurrentTile.Y;
            switch (action)
            {
                case Actions.ShootRight:
                case Actions.EchoRight:
                    return GetTileTo(() => ++x, () => y);
                case Actions.ShootDown:
                case Actions.EchoDown:
                    return GetTileTo(() => x, () => ++y);
                case Actions.ShootLeft:
                case Actions.EchoLeft:
                    return GetTileTo(() => --x, () => y);
                case Actions.ShootUp:
                case Actions.EchoUp:
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
            } while (tile.Item == Items.Nothing);

            return tile;
        }

        private static MapTile OuterWall(int x, int y)
        {
            return new MapTile(x, y){Item = Items.SteelWall};
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
