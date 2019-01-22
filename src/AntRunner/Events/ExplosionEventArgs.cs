using System;
using AntRunner.Models;

namespace AntRunner.Events
{
    public class ExplosionEventArgs : EventArgs
    {
        public MapTile Tile { get; }

        public ExplosionEventArgs(MapTile tile)
        {
            Tile = tile;
        }
    }
}
