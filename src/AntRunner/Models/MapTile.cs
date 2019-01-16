using AntRunner.Interface;

namespace AntRunner.Models
{
    public class MapTile : NotifyBaseModel
    {
        private Item _item;
        public Item Item
        {
            get => _item;
            set => SetValue(ref _item, value);
        }

        public AntWrapper OccupiedBy { get; set; }

        public int X { get; }
        public int Y { get; }

        public MapTile(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}
