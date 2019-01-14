namespace AntRunner.Interface
{
    public readonly struct GameState
    {
        public long TickNumber { get; }
        public bool HasFlag { get; }
        public int FlagX { get; }
        public int FlagY { get; }
        public Colors AntWithFlag { get; }

        public EchoResponse Response { get; }
        public GameEvent Event { get; }
        
        public GameState(long tickNumber)
        {
            TickNumber = tickNumber;
            FlagX = -1;
            FlagY = -1;
            HasFlag = false;
            AntWithFlag = Colors.None;
            Response = null;
            Event = GameEvent.Nothing;
        }

        public GameState(long tickNumber, bool hasFlag, int flagX, int flagY, Colors antWithFlag)
        {
            TickNumber = tickNumber;
            HasFlag = hasFlag;
            FlagX = flagX;
            FlagY = flagY;
            AntWithFlag = antWithFlag;
            Response = null;
            Event = GameEvent.Nothing;
        }

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
