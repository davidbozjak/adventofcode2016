using System.Drawing;

namespace SantasToolbox
{
    public class Tile : IWorldObject, INode, IEquatable<Tile>
    {
        public Point Position { get; }

        public virtual char CharRepresentation => this.IsTraversable ? '.' : '#';

        public int Z => 0;

        public bool IsTraversable { get; }

        private readonly Cached<IEnumerable<Tile>> cachedNeighbours;

        public IEnumerable<Tile> TraversibleNeighbours => this.cachedNeighbours.Value;

        public int Cost => 1;

        public Tile(int x, int y, bool isTraversable, Func<Tile, IEnumerable<Tile>> fillTraversibleNeighboursFunc)
        {
            Position = new Point(x, y);
            this.IsTraversable = isTraversable;
            this.cachedNeighbours = new Cached<IEnumerable<Tile>>(() => fillTraversibleNeighboursFunc(this));
        }

        public bool Equals(Tile? other)
        {
            if (other == null) return false;
            return base.Equals(other);
        }
    }

    public class TileWorld : IWorld
    {
        private readonly List<Tile> allTiles = new();

        public IEnumerable<IWorldObject> WorldObjects => this.allTiles;

        public TileWorld(IEnumerable<string> map, Func<int, int, char, Func<Tile, IEnumerable<Tile>>, Tile> tileCreatingFunc)
        {
            int y = 0;
            foreach (var line in map)
            {
                for (int x = 0; x < line.Length; x++)
                {
                    char c = line[x];

                    allTiles.Add(tileCreatingFunc(x, y, c, GetTraversibleNeighboursOfTile));
                }
                y++;
            }
        }

        private IEnumerable<Tile> GetTraversibleNeighboursOfTile(Tile tile)
        {
            return this.allTiles.Where(w => w.IsTraversable && tile.Position.IsNeighbour(w.Position));
        }
    }
}
