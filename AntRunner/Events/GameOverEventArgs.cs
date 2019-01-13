using System;
using AntRunner.Interface;

namespace AntRunner.Events
{
    public class GameOverEventArgs : EventArgs
    {
        public bool HasWinner { get; }

        public Colors Color { get; }
        public string Name { get; }

        public GameOverEventArgs()
        {
            
        }

        public GameOverEventArgs(Colors color, string name)
        {
            HasWinner = true;
            Color = color;
            Name = name;
        }
    }
}
