namespace SantasToolbox
{
    public class SimpleWorld<T> : IWorld
        where T : IWorldObject
    {
        public IEnumerable<IWorldObject> WorldObjects => this.worldObjects.Cast<IWorldObject>();
        public IEnumerable<T> WorldObjectsT => this.worldObjects;

        public int MaxX => this.worldObjects.Max(w => w.Position.X);
        public int MaxY => this.worldObjects.Max(w => w.Position.Y);

        public int MinX => this.worldObjects.Min(w => w.Position.X);
        public int MinY => this.worldObjects.Min(w => w.Position.Y);

        public int Width => this.MaxX - this.MinX + 1;
        public int Height => this.MaxY - this.MinY + 1;

        private readonly List<T> worldObjects;
        private readonly Dictionary<(int x, int y), T> spatialWorldObjects;

        public SimpleWorld(IEnumerable<T> objects)
        {
            this.worldObjects = objects.ToList();
            spatialWorldObjects = objects.ToDictionary(w => (w.Position.X, w.Position.Y));
        }

        public T GetObjectAt(int x, int y)
            => this.spatialWorldObjects[(x, y)];
    }
}
