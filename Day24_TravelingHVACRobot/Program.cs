using System.Drawing;

var mapLines = new InputProvider<string?>("Input.txt", GetString).Where(w => w != null).Cast<string>().ToList();

var world = new TileWorld(mapLines, CreateTile);

var printer = new WorldPrinter();
printer.Print(world);

static Tile CreateTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> fillNeighboursFunc)
{
    if (char.IsDigit(c))
    {
        return new PointOfInterestTile(c - '0', x, y, true, fillNeighboursFunc);
    }
    else
    {
        return new Tile(x, y, c == '.', fillNeighboursFunc);
    }
}

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}

class Tile : IWorldObject
{
    public Point Position { get; }

    public virtual char CharRepresentation => this.IsTraversable ? '.' : '#';

    public int Z => 0;

    public bool IsTraversable { get; }

    private readonly Cached<IEnumerable<Tile>> cachedNeighbours;

    public IEnumerable<Tile> Neighbours => this.cachedNeighbours.Value;

    public Tile(int x, int y, bool isTraversable, Func<Tile, IEnumerable<Tile>> fillNeighboursFunc)
    {
        Position = new Point(x, y);
        this.IsTraversable = isTraversable;
        this.cachedNeighbours = new Cached<IEnumerable<Tile>>(() => fillNeighboursFunc(this));
    }
}

class PointOfInterestTile : Tile
{
    public int NumberOfInterest { get; }

    override public char CharRepresentation => (char)('0' + NumberOfInterest);

    public PointOfInterestTile(int numberOfInterest, int x, int y, bool isTraversable, Func<Tile, IEnumerable<Tile>> fillNeighboursFunc)
        : base(x, y, isTraversable, fillNeighboursFunc)
    {
        this.NumberOfInterest = numberOfInterest;
    }
}

class TileWorld : IWorld
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

                allTiles.Add(tileCreatingFunc(x, y, c, GetNeighboursOfTile));
            }
            y++;
        }
    }

    private IEnumerable<Tile> GetNeighboursOfTile(Tile tile)
    {
        return this.allTiles.Where(w => tile.Position.IsNeighbour(w.Position));
    }
}