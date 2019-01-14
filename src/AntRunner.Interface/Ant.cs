using System.IO;

namespace AntRunner.Interface
{
    public abstract class Ant
    {
        public abstract string Name { get; }

        public Actions Action;
        public abstract void Initialize(int mapWidth, int mapHeight, Colors color, int startX, int startY);
        public abstract void Tick(GameState state);

        public virtual string IconResource => "AntRunner.Interface.Icon.png";

        public virtual Stream Icon => GetType().Assembly.GetManifestResourceStream(IconResource) ?? GetType().Assembly.GetManifestResourceStream("AntRunner.Interface.Icon.png");
    }
}
