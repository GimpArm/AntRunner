using System;
using AntRunner.Models;

namespace AntRunner.Events
{
    public class ShootEventHandler : EventArgs
    {
        public int Distance { get; }
        public Direction Direction { get; }

        public ShootEventHandler(int distance, Direction direction)
        {
            Distance = distance;
            Direction = direction;
        }
    }
}
