using System;
using System.Linq;
using System.Reflection;
using System.Windows;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public static class Utilities
    {
        public static readonly Random Random = new Random(DateTime.Now.Millisecond);

        public static Ant LoadAnt(string filename)
        {
            try
            {
                var loadedAssembly = Assembly.LoadFrom(filename);

                var antType = loadedAssembly.GetTypes().FirstOrDefault(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(Ant)));
                if (antType == null)
                {
                    throw new Exception("Could not find class derived from AntRunner.Interface.Ant");
                }

                if (!(Activator.CreateInstance(antType) is Ant ant))
                {
                    throw new Exception($"Could not initialize Ant class {antType.Name}");
                }

                return ant;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }
        }

        public static Items ColorToAntItem(Colors color)
        {
            switch (color)
            {
                case Colors.Red:
                    return Items.RedAnt;
                case Colors.Blue:
                    return Items.BlueAnt;
                case Colors.Green:
                    return Items.GreenAnt;
                case Colors.Orange:
                    return Items.OrangeAnt;
                case Colors.Pink:
                    return Items.PinkAnt;
                case Colors.Yellow:
                    return Items.YellowAnt;
                case Colors.Gray:
                    return Items.GrayAnt;
                case Colors.White:
                    return Items.WhiteAnt;
                default:
                    return Items.Nothing;
            }
        }

        public static Items ColorToAntHomeItem(Colors color)
        {
            switch (color)
            {
                case Colors.Red:
                    return Items.RedHome;
                case Colors.Blue:
                    return Items.BlueHome;
                case Colors.Green:
                    return Items.GreenHome;
                case Colors.Orange:
                    return Items.OrangeHome;
                case Colors.Pink:
                    return Items.PinkHome;
                case Colors.Yellow:
                    return Items.YellowHome;
                case Colors.Gray:
                    return Items.GrayHome;
                case Colors.White:
                    return Items.WhiteHome;
                default:
                    return Items.Nothing;
            }
        }

        public static GameEvent ShotDirectionToEvent(Actions a)
        {
            switch (a)
            {
                case Actions.ShootRight:
                    return GameEvent.ImpactDamageLeft;
                case Actions.ShootDown:
                    return GameEvent.ImpactDamageUp;
                case Actions.ShootLeft:
                    return GameEvent.ShotDamageRight;
                case Actions.ShootUp:
                    return GameEvent.ShotDamageDown;
                default:
                    return GameEvent.Nothing;
            }
        }

        public static int CalculateDistance(AntWrapper ant, MapTile tile)
        {
            return CalculateDistance(ant, tile.X, tile.Y);
        }

        public static int CalculateDistance(AntWrapper ant, int x, int y)
        {
            return Math.Abs(x - ant.CurrentTile.X + y - ant.CurrentTile.Y);
        }
    }
}
