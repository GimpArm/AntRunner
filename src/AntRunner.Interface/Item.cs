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
        Empty = 0,

        /// <summary>
        /// Unbreakable steel wall.
        /// </summary>
        SteelWall = 1,

        /// <summary>
        /// Brick wall which may be destroyed with a laser shot.
        /// </summary>
        BrickWall = 2,

        /// <summary>
        /// Bomb that will damage the ant if stepped on. Can be shot with the laser to destroy
        /// See <see cref="DamageValues.Bomb"/>
        /// </summary>
        Bomb = 3,

        /// <summary>
        /// Bomb Power-up, adds bombs to the ant's inventory.
        /// See <see cref="ItemBonusValues.Bomb"/>
        /// </summary>
        PowerUpBomb = 4,

        /// <summary>
        /// Health Power-up, adds more health level to the ant, maximum 100.
        /// See <see cref="ItemBonusValues.Health"/>
        /// </summary>
        PowerUpHealth = 5,

        /// <summary>
        /// Shield Power-up, adds more shield level to the ant, maximum 100.
        /// See <see cref="ItemBonusValues.Shield"/>
        /// </summary>
        PowerUpShield = 6,

        /// <summary>
        /// The red ant.
        /// </summary>
        RedAnt = 7,

        /// <summary>
        /// The blue ant.
        /// </summary>
        BlueAnt = 8,

        /// <summary>
        /// The green ant.
        /// </summary>
        GreenAnt = 9,

        /// <summary>
        /// The orange ant.
        /// </summary>
        OrangeAnt = 10,

        /// <summary>
        /// The pink ant.
        /// </summary>
        PinkAnt = 11,

        /// <summary>
        /// The yellow ant.
        /// </summary>
        YellowAnt = 12,

        /// <summary>
        /// The gray ant.
        /// </summary>
        GrayAnt = 13,

        /// <summary>
        /// The white ant.
        /// </summary>
        WhiteAnt = 14,

        /// <summary>
        /// The home for the red ant.
        /// </summary>
        RedHome = 15,

        /// <summary>
        /// The home for the blue ant.
        /// </summary>
        BlueHome = 16,

        /// <summary>
        /// The home for the green ant.
        /// </summary>
        GreenHome = 17,

        /// <summary>
        /// The home for the orange ant.
        /// </summary>
        OrangeHome = 18,

        /// <summary>
        /// The home for the pink ant.
        /// </summary>
        PinkHome = 19,

        /// <summary>
        /// The home for the yellow ant.
        /// </summary>
        YellowHome = 20,

        /// <summary>
        /// The home for the gray ant.
        /// </summary>
        GrayHome = 21,

        /// <summary>
        /// The home for the white ant.
        /// </summary>
        WhiteHome = 22,

        /// <summary>
        /// The flag, pick this up and bring it to the correct color home.
        /// </summary>
        Flag = 23
    }
}
