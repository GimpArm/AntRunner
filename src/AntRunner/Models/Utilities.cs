using System;
using AntRunner.Interface;

namespace AntRunner.Models
{
    public static class Utilities
    {
        public static readonly Random Random = new Random(DateTime.Now.Millisecond);


        public static Item ColorToAntItem(ItemColor color)
        {
            switch (color)
            {
                case ItemColor.Red:
                    return Item.RedAnt;
                case ItemColor.Blue:
                    return Item.BlueAnt;
                case ItemColor.Green:
                    return Item.GreenAnt;
                case ItemColor.Orange:
                    return Item.OrangeAnt;
                case ItemColor.Pink:
                    return Item.PinkAnt;
                case ItemColor.Yellow:
                    return Item.YellowAnt;
                case ItemColor.Gray:
                    return Item.GrayAnt;
                case ItemColor.White:
                    return Item.WhiteAnt;
                default:
                    return Item.Empty;
            }
        }

        public static Item ColorToAntHomeItem(ItemColor color)
        {
            switch (color)
            {
                case ItemColor.Red:
                    return Item.RedHome;
                case ItemColor.Blue:
                    return Item.BlueHome;
                case ItemColor.Green:
                    return Item.GreenHome;
                case ItemColor.Orange:
                    return Item.OrangeHome;
                case ItemColor.Pink:
                    return Item.PinkHome;
                case ItemColor.Yellow:
                    return Item.YellowHome;
                case ItemColor.Gray:
                    return Item.GrayHome;
                case ItemColor.White:
                    return Item.WhiteHome;
                default:
                    return Item.Empty;
            }
        }

        public static GameEvent ShotDirectionToEvent(AntAction a)
        {
            switch (a)
            {
                case AntAction.ShootRight:
                    return GameEvent.ImpactDamageLeft;
                case AntAction.ShootDown:
                    return GameEvent.ImpactDamageUp;
                case AntAction.ShootLeft:
                    return GameEvent.ShotDamageRight;
                case AntAction.ShootUp:
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
