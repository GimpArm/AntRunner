using AntRunner.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AntRunner.Game.Interface
{
    public class AntState
    {
        public Guid ID { get; set; }

        public string Name { get; set; }

        public ItemColor Color { get; set; }

        public int PositionX { get; set; }
        public int PositionY { get; set; }

        public int Health { get; set; }
        public int Shields { get; set; }

        public AntAction LastAction { get; set; }   
    }
}
