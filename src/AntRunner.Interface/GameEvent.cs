using System;

namespace AntRunner.Interface
{
    [Flags]
    public enum GameEvent
    {
        Nothing = 0x00000,
        CollisionDamage = 0x00001,
        ImpactDamageRight = 0x00002,
        ImpactDamageDown = 0x00004,
        ImpactDamageLeft = 0x00008,
        ImpactDamageUp = 0x00010,
        ShotDamageRight = 0x00020,
        ShotDamageDown = 0x00040,
        ShotDamageLeft = 0x00080,
        ShotDamageUp = 0x00100,
        BombDamage = 0x000200,
        PickUpBomb = 0x00400,
        PickUpShield = 0x00800,
        PickUpHealth = 0x01000,
        PickUpFlag = 0x02000,
        HasFlag = 0x04000,
        Dead = 0x08000,
        GameOver = 0x10000
    }
}
