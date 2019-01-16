using System.IO;

namespace AntRunner.Interface
{
    /// <summary>
    /// Base class for every ant, inherit from this class to create an ant.
    /// </summary>
    public abstract class Ant
    {
        /// <summary>
        /// Readonly property for the name of the ant.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Overridable string to return the name of an embedded resource for use the ant's Flag
        /// </summary>
        public virtual string FlagResource => "AntRunner.Interface.Flag.png";

        /// <summary>
        /// Overridable Stream to return binary data for use by BitmapFrame.Create() of the ant's Flag.
        /// </summary>
        public virtual Stream Flag => GetType().Assembly.GetManifestResourceStream(FlagResource) ?? GetType().Assembly.GetManifestResourceStream("AntRunner.Interface.Flag.png");

        /// <summary>
        /// Action which will be performed for the end of the current tick cycle. Will be set to Wait after being read.
        /// </summary>
        public AntAction Action;

        /// <summary>
        /// Initialize method called once before the start of each game.
        /// </summary>
        /// <param name="mapWidth">The total width of the map.</param>
        /// <param name="mapHeight">The total height of the map.</param>
        /// <param name="antColor">The color the ant was assigned.</param>
        /// <param name="startX">The X coordinate value where the ant starts.</param>
        /// <param name="startY">The Y coordinate value where the ant starts.</param>
        public abstract void Initialize(int mapWidth, int mapHeight, ItemColor antColor, int startX, int startY);

        /// <summary>
        /// Method called to begin processing for each turn.
        /// </summary>
        /// <param name="state">GameState object of relevant information that occurred as a result of the GameActions of the previous Tick.</param>
        public abstract void Tick(GameState state);

    }
}
