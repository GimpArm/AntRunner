namespace AntRunner.Interface
{
    /// <summary>
    /// Response item when an Echo action is made.
    /// </summary>
    public class EchoResponse
    {
        /// <summary>
        /// How many squares away is the item.
        /// </summary>
        public int Distance { get; }

        /// <summary>
        /// What item is there.
        /// </summary>
        public Item Item { get; }

        /// <summary>
        /// Response item when an Echo action is made.
        /// </summary>
        /// <param name="distance">How many squares away is the item.</param>
        /// <param name="item">What item is there.</param>
        public EchoResponse(int distance, Item item)
        {
            Distance = distance;
            Item = item;
        }
    }
}