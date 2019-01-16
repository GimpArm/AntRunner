namespace AntRunner.Interface
{
    /// <summary>
    /// Enum of available actions an ant can make per tick.
    /// </summary>
    public enum AntAction
    {
        /// <summary>
        /// Ant does nothing.
        /// </summary>
        Wait = 0,

        /// <summary>
        /// Ant attempts to move one space to the right.
        /// </summary>
        MoveRight = 1,

        /// <summary>
        /// Ant attempts to move one space below.
        /// </summary>
        MoveDown = 2,

        /// <summary>
        /// Ant attempts to move one space to the left.
        /// </summary>
        MoveLeft = 3,

        /// <summary>
        /// Ant attempts to move one space above.
        /// </summary>
        MoveUp = 4,

        /// <summary>
        /// Ant performs an echo to the right. The response will come in the GameState of the next Tick() call.
        /// See <see cref="EchoResponse"/>
        /// </summary>
        EchoRight = 5,

        /// <summary>
        /// Ant performs an echo below. The response will come in the GameState of the next Tick() call.
        /// See <see cref="EchoResponse"/>
        /// </summary>
        EchoDown = 6,

        /// <summary>
        /// Ant performs an echo to the left. The response will come in the GameState of the next Tick() call.
        /// See <see cref="EchoResponse"/>
        /// </summary>
        EchoLeft = 7,

        /// <summary>
        /// Ant performs an echo above. The response will come in the GameState of the next Tick() call.
        /// See <see cref="EchoResponse"/>
        /// </summary>
        EchoUp = 8,

        /// <summary>
        /// Ant turns on it's shield if it has any.
        /// For every 4 ticks the shield is on, it will lose 1 point.
        /// </summary>
        ShieldOn = 9,

        /// <summary>
        /// Ant turns off it's shield.
        /// </summary>
        ShieldOff = 10,

        /// <summary>
        /// Ant drops a bomb at the current position if it has any.
        /// </summary>
        DropBomb = 11,

        /// <summary>
        /// Ant shoot its laser in a straight line to the right.
        /// </summary>
        ShootRight = 12,

        /// <summary>
        /// Ant shoot its laser in a straight line below.
        /// </summary>
        ShootDown = 13,

        /// <summary>
        /// Ant shoot its laser in a straight line to the left.
        /// </summary>
        ShootLeft = 14,

        /// <summary>
        /// Ant shoot its laser in a straight line above.
        /// </summary>
        ShootUp = 15
    }
}
