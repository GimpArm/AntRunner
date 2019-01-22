using System;

namespace AntRunner.Interface
{
    /// <summary>
    /// Enum flags of possible events that can occur as a result of all ants' GameAction. Multiple events may occur at once.
    /// </summary>
    [Flags]
    public enum GameEvent
    {
        /// <summary>
        /// Nothing has occurred.
        /// </summary>
        Nothing = 0x00000,

        /// <summary>
        /// Ant ran into an object when it attempted to move and incurred damage.  The move was unsuccessful and the ant remains at its current location.
        /// See <see cref="DamageValues.Collision"/>
        /// </summary>
        CollisionDamage = 0x00001,

        /// <summary>
        /// Another ant ran into or rammed the ant from the right and incurred damage.
        /// See <see cref="DamageValues.Impact"/>
        /// </summary>
        ImpactDamageRight = 0x00002,

        /// <summary>
        /// Another ant ran into or rammed the ant from below and incurred damage.
        /// See <see cref="DamageValues.Impact"/>
        /// </summary>
        ImpactDamageDown = 0x00004,

        /// <summary>
        /// Another ant ran into or rammed the ant from the left and incurred damage.
        /// See <see cref="DamageValues.Impact"/>
        /// </summary>
        ImpactDamageLeft = 0x00008,

        /// <summary>
        /// Another ant ran into or rammed the ant from above and incurred damage.
        /// See <see cref="DamageValues.Impact"/>
        /// </summary>
        ImpactDamageUp = 0x00010,

        /// <summary>
        /// Ant was shot by a laser and incurred damage from the right.
        /// See <see cref="DamageValues.Shot"/>
        /// </summary>
        /// 
        ShotDamageRight = 0x00020,

        /// <summary>
        /// Ant was shot by a laser and incurred damage from below.
        /// See <see cref="DamageValues.Shot"/>
        /// </summary>
        ShotDamageDown = 0x00040,

        /// <summary>
        /// Ant was shot by a laser and incurred damage from the left.
        /// See <see cref="DamageValues.Shot"/>
        /// </summary>
        ShotDamageLeft = 0x00080,

        /// <summary>
        /// Ant was shot by a laser and incurred damage from above.
        /// See <see cref="DamageValues.Shot"/>
        /// </summary>
        ShotDamageUp = 0x00100,

        /// <summary>
        /// Ant walked over a bomb and incurred damage.
        /// See <see cref="DamageValues.Bomb"/>
        /// </summary>
        BombDamage = 0x000200,

        /// <summary>
        /// Ant picked up a Bomb Power-up.
        /// See <see cref="ItemBonusValues.Bomb"/>
        /// </summary>
        PickUpBomb = 0x00400,

        /// <summary>
        /// Ant picked up a Shield Power-up.
        /// See <see cref="ItemBonusValues.Shield"/>
        /// </summary>
        PickUpShield = 0x00800,

        /// <summary>
        /// Ant picked up a Health Power-up.
        /// See <see cref="ItemBonusValues.Health"/>
        /// </summary>
        PickUpHealth = 0x01000,

        /// <summary>
        /// Ant picked up the Flag. Run to the correct color home!
        /// </summary>
        PickUpFlag = 0x02000,

        /// <summary>
        /// Ant has died and will no longer be getting Tick() calls.
        /// </summary>
        Dead = 0x04000,

        /// <summary>
        /// The game is over, either an ant successfully retrieved the flag or all ants have died.
        /// </summary>
        GameOver = 0x08000
    }
}
