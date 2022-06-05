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

        var path = AStarPathfinder.FindPath<Tile>(point1, point2, t => point2.Position.Distance(t.Position), t => t.TraversibleNeighbours);
        var distance = path.Count - 1;

        distances[(point1, point2)] = distance;
        distances[(point2, point1)] = distance;
    }
}

var orderedPoints = pointsOfInterest.OrderBy(w => w.NumberOfInterest);
var pointZero = orderedPoints.First();
var pointsToVisit = orderedPoints.Skip(1).ToList();

Console.WriteLine($"Part 1: {GetMinDistance(pointZero, pointsToVisit, _ => 0)}");
Console.WriteLine($"Part 2: {GetMinDistance(pointZero, pointsToVisit, t => distances[(t, pointZero)])}");

int GetMinDistance(PointOfInterestTile tile, List<PointOfInterestTile> tilesToVisit, Func<PointOfInterestTile, int> endFunc)
{
    if (tilesToVisit.Count == 0) return endFunc(tile);

    int min = int.MaxValue;

    foreach (var targetTile in tilesToVisit)
    {
        var distance = distances[(tile, targetTile)];

        var totalDistance = distance + GetMinDistance(targetTile, tilesToVisit.Where(w => w != targetTile).ToList(), endFunc);

        if (totalDistance < min)
        {
            min = totalDistance;
        }
    }

    return min;
}

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