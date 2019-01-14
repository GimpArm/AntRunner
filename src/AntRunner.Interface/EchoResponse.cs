namespace AntRunner.Interface
{
    public class EchoResponse
    {
        public int Distance { get; }
        public Items Item { get; }

        public EchoResponse(int distance, Items item)
        {
            Distance = distance;
            Item = item;
        }
    }
}
