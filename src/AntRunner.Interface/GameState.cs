namespace AntRunner.Interface
{
    public readonly struct GameState
    {
        /// <summary>
        /// Long value of the tick turn, starts with 0 and each Tick() call is increased by 1.
        /// See <see cref="Ant.Tick"/>
        /// </summary>
        public long TickNumber { get; }

        /// <summary>
        /// Boolean value if an ant has the flag.
        /// </summary>
        public bool HasFlag { get; }

        /// <summary>
        /// If an ant has the flag, this is their current X position on the map. If no ants have the flag then this value is -1.
        /// </summary>
        public int FlagX { get; }

        /// <summary>
        /// If an ant has the flag, this is their current Y position on the map. If no ants have the flag then this value is -1.
        /// </summary>
        public int FlagY { get; }

        /// <summary>
        /// If an ant has the flag, this is the color of that ant. If no ants have the flag then this value is ItemColor.None.
        /// </summary>
        public ItemColor AntWithFlag { get; }

        /// <summary>
        /// Echo response of the previous Tick GameAction. If previous Tick was not an echo action then this is null.
        /// See <see cref="AntAction.EchoRight"/>
        /// <seealso cref="AntAction.EchoLeft"/>
        /// <seealso cref="AntAction.EchoUp"/>
        /// <seealso cref="AntAction.EchoDown"/>
        /// </summary>
        public EchoResponse Response { get; }

        /// <summary>
        /// Flag enum of which events occurred due to the previous Tick() GameAction.
        /// </summary>
        public GameEvent Event { get; }
        
        /// <summary>
        /// Constructor when there is no GameEvent, meaning nothing happened.
        /// </summary>
        /// <param name="tickNumber">Tick number of the current Tick cycle.</param>
        public GameState(long tickNumber)
        {
            TickNumber = tickNumber;
            FlagX = -1;
            FlagY = -1;
            HasFlag = false;
            AntWithFlag = ItemColor.None;
            Response = null;
            Event = GameEvent.Nothing;
        }

        /// <summary>
        /// Constructor when an ant has the flag.
        /// </summary>
        /// <param name="tickNumber">Tick number of the current Tick cycle.</param>
        /// <param name="flagX">The current X position on the map of the ant with the flag.</param>
        /// <param name="flagY">The current Y position on the map of the ant with the flag.</param>
        /// <param name="antWithFlag">The ItemColor of the ant with the flag.</param>
        public GameState(long tickNumber, int flagX, int flagY, ItemColor antWithFlag)
        {
            TickNumber = tickNumber;
            HasFlag = true;
            FlagX = flagX;
            FlagY = flagY;
            AntWithFlag = antWithFlag;
            Response = null;
            Event = GameEvent.Nothing;
        }

        /// <summary>
        /// Constructor for a personalized GameState for each ant.
        /// </summary>
        /// <param name="state">General GameState object to copy values.</param>
        /// <param name="event">Flag enum of which events occurred due to the previous Tick() GameAction.</param>
        /// <param name="response">Echo response of the previous Tick GameAction. If previous Tick was not an echo action then this is null.</param>
        public GameState(GameState state, GameEvent @event, EchoResponse response = null)
        {
            TickNumber = state.TickNumber;
            HasFlag = state.HasFlag;
            FlagX = state.FlagX;
            FlagY = state.FlagY;
            AntWithFlag = state.AntWithFlag;
            Event = @event;
            Response = response;
        }
    }
}
