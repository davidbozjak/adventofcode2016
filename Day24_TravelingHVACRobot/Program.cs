using System.Drawing;
using System.Diagnostics;

var mapLines = new InputProvider<string?>("Input.txt", GetString).Where(w => w != null).Cast<string>().ToList();

List<PointOfInterestTile> pointsOfInterest = new();
var world = new TileWorld(mapLines, CreateTile);

var printer = new WorldPrinter();
printer.Print(world);

Dictionary<(PointOfInterestTile, PointOfInterestTile), int> distances = new();

for (int i = 0; i < pointsOfInterest.Count; i++)
{
    var point1 = pointsOfInterest[i];
    for (int j = i + 1; j < pointsOfInterest.Count; j++)
    {
        var point2 = pointsOfInterest[j];

        var path = AStarPathfinder.FindPath<Tile>(point1, point2, t => point2.Position.Distance(t.Position), t => t.Neighbours);
        var distance = path.Count - 1;

        distances[(point1, point2)] = distance;
        distances[(point2, point1)] = distance;
    }
}

Console.WriteLine(distances);

Tile CreateTile(int x, int y, char c, Func<Tile, IEnumerable<Tile>> fillNeighboursFunc)
{
    if (char.IsDigit(c))
    {
        var point = new PointOfInterestTile(c - '0', x, y, true, fillNeighboursFunc);
        pointsOfInterest.Add(point);
        return point;
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

class Tile : IWorldObject, INode, IEquatable<Tile>
{
    public Point Position { get; }

    public virtual char CharRepresentation => this.IsTraversable ? '.' : '#';

    public int Z => 0;

    public bool IsTraversable { get; }

    private readonly Cached<IEnumerable<Tile>> cachedNeighbours;

    public IEnumerable<Tile> Neighbours => this.cachedNeighbours.Value;

    public int Cost => 1;

    public Tile(int x, int y, bool isTraversable, Func<Tile, IEnumerable<Tile>> fillNeighboursFunc)
    {
        Position = new Point(x, y);
        this.IsTraversable = isTraversable;
        this.cachedNeighbours = new Cached<IEnumerable<Tile>>(() => fillNeighboursFunc(this));
    }

    public bool Equals(Tile? other)
    {
        if (other == null) return false;
        return base.Equals(other);
    }
}

[DebuggerDisplay("{CharRepresentation}")]
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