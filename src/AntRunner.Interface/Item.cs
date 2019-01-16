namespace AntRunner.Interface
{
    /// <summary>
    /// Enum of possible items on a map position.
    /// </summary>
    public enum Item
    {
        /// <summary>
        /// Nothing at all, aka empty.
        /// </summary>
        Empty = 0x000000,

        /// <summary>
        /// Unbreakable steel wall.
        /// </summary>
        SteelWall = 0x000001,

        /// <summary>
        /// Brick wall which may be destroyed with a laser shot.
        /// </summary>
        BrickWall = 0x000002,

        /// <summary>
        /// Bomb that will damage the ant if stepped on. Can do shot with the laser to remove.
        /// See <see cref="DamageValues.Bomb"/>
        /// </summary>
        Bomb = 0x000004,

        /// <summary>
        /// Bomb Power-up, adds bomb to the ant's inventory.
        /// See <see cref="ItemBonusValues.Bomb"/>
        /// </summary>
        PowerUpBomb = 0x000008,

        /// <summary>
        /// Health Power-up, adds more health level to the ant, maximum 100.
        /// See <see cref="ItemBonusValues.Health"/>
        /// </summary>
        PowerUpHealth = 0x000010,

        /// <summary>
        /// Shield Power-up, adds more shield level to the ant, maximum 100.
        /// See <see cref="ItemBonusValues.Shield"/>
        /// </summary>
        PowerUpShield = 0x000020,

        /// <summary>
        /// The red ant.
        /// </summary>
        RedAnt = 0x000040,

        /// <summary>
        /// The blue ant.
        /// </summary>
        BlueAnt = 0x000080,

        /// <summary>
        /// The green ant.
        /// </summary>
        GreenAnt = 0x000100,

        /// <summary>
        /// The orange ant.
        /// </summary>
        OrangeAnt = 0x000200,

        /// <summary>
        /// The pink ant.
        /// </summary>
        PinkAnt = 0x000400,

        /// <summary>
        /// The yellow ant.
        /// </summary>
        YellowAnt = 0x000800,

        /// <summary>
        /// The gray ant.
        /// </summary>
        GrayAnt = 0x001000,

        /// <summary>
        /// The white ant.
        /// </summary>
        WhiteAnt = 0x002000,

        /// <summary>
        /// The home for the red ant.
        /// </summary>
        RedHome = 0x004000,

        /// <summary>
        /// The home for the blue ant.
        /// </summary>
        BlueHome = 0x008000,

        /// <summary>
        /// The home for the green ant.
        /// </summary>
        GreenHome = 0x010000,

        /// <summary>
        /// The home for the orange ant.
        /// </summary>
        OrangeHome = 0x020000,

        /// <summary>
        /// The home for the pink ant.
        /// </summary>
        PinkHome = 0x040000,

        /// <summary>
        /// The home for the yellow ant.
        /// </summary>
        YellowHome = 0x080000,

        /// <summary>
        /// The home for the gray ant.
        /// </summary>
        GrayHome = 0x100000,

        /// <summary>
        /// The home for the white ant.
        /// </summary>
        WhiteHome = 0x200000,

        /// <summary>
        /// The flag, pick this up and bring it to the correct color home.
        /// </summary>
        Flag = 0x400000
    }
}
