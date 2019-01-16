namespace AntRunner.Interface
{
    /// <summary>
    /// Static class of constant values used when assigning power-ups to an ant.
    /// </summary>
    public static class ItemBonusValues
    {
        /// <summary>
        /// Amount of health that is restored when a health power-up is acquired.
        /// See <see cref="Item.PowerUpHealth"/>
        /// </summary>
        public const int Health = 25;

        /// <summary>
        /// Amount of shield that is restored when a shield power-up is acquired.
        /// See <see cref="Item.PowerUpShield"/>
        /// </summary>
        public const int Shield = 25;

        /// <summary>
        /// Amount of bombs added to an ant's inventory when a bomb power-up is acquired.
        /// See <see cref="Item.PowerUpBomb"/>
        /// </summary>
        public const int Bomb = 4;
    }
}
