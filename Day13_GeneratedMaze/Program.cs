using System.Drawing;

//test input:
//int magicNumber = 10;
//real input:
int magicNumber = 1362;
Room.WorldGeneratingFunc = (x, y) =>
{
    if (x < 0) throw new Exception();
    if (y < 0) throw new Exception();

    int number = x * x + 3 * x + 2 * x * y + y + y * y + magicNumber;
    var binary = Convert.ToString(number, 2);
    return binary.Count(w => w == '1') % 2 == 0;
};

var world = new World();

var start = world.GetRoomAtLocation(1, 1);
var end = world.GetRoomAtLocation(31, 39);

var path = AStarPathfinder.FindPath(
    start,
    end,
    w => Math.Abs(w.Position.X - start.Position.X) + Math.Abs(w.Position.Y - start.Position.Y),
    room => world.GetNeighbouringRooms(room));

Console.WriteLine($"Part 1: {path.Count - 1}");

world = new World();
HashSet<Room> reachableRooms = new();
HashSet<(Room, int, Room)> visited = new();

AddReachableRooms(world.GetRoomAtLocation(1, 1), world, reachableRooms, visited, 50);
PrintPath(world, reachableRooms.ToList());
Console.WriteLine($"Part 2: {reachableRooms.Count}");

static void AddReachableRooms(Room room, World world, HashSet<Room> reachableRooms, HashSet<(Room, int, Room)> visited, int stepsLeft)
{
    if (stepsLeft == 0) return;

    foreach (var n in world.GetNeighbouringRooms(room))
    {
        if (visited.Contains((n, stepsLeft, room))) 
            continue;

        visited.Add((n, stepsLeft, room));
        reachableRooms.Add(n);
        
        AddReachableRooms(n, world, reachableRooms, visited, stepsLeft - 1);
    }
}

static void PrintPath(World world, List<Room> path)
{
    path.ForEach(w => w.SetCharRepresentationOverride('O'));

    var printer = new WorldPrinter();
    printer.Print(world);

    path.ForEach(w => w.ResetCharRepresentationOverride());
}

public class Room : IWorldObject, INode, IEquatable<Room>
{
    public static Func<int, int, bool> WorldGeneratingFunc { get; set; }

    public Point Position { get; }

    public bool IsTraversable { get; }

    private char? overrideCharRepresentaion;
    public char CharRepresentation => overrideCharRepresentaion ?? (this.IsTraversable ? '.' : '#');

    public int Z => 1;

    public int Cost => 1;

    public Room(int x, int y)
    {
        this.Position = new Point(x, y);
        this.IsTraversable = WorldGeneratingFunc(x, y);
    }

    public void SetCharRepresentationOverride(char c)
    {
        this.overrideCharRepresentaion = c;
    }

    public void ResetCharRepresentationOverride()
    {
        this.overrideCharRepresentaion = null;
    }

    public bool Equals(Room? other)
    {
        if (other == null) return false;
        else return this.GetHashCode() == other.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        else if (obj is Room room)
        {
            return this.Equals(room);
        }
        else return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}

class World : IWorld
{
    private readonly UniqueFactory<(int x, int y), Room> rooms = new(w => new Room(w.x, w.y));

    public IEnumerable<IWorldObject> WorldObjects => rooms.AllCreatedInstances;

    public IEnumerable<Room> GetNeighbouringRooms(Room room)
    {
        if (room.Position.Y > 0)
        {
            var above = rooms.GetOrCreateInstance((room.Position.X, room.Position.Y - 1));
            if (above.IsTraversable) yield return above;
        }
        if (room.Position.X > 0)
        {
            var left = rooms.GetOrCreateInstance((room.Position.X - 1, room.Position.Y));
            if (left.IsTraversable) yield return left;
        }

        var below = rooms.GetOrCreateInstance((room.Position.X, room.Position.Y + 1));
        if (below.IsTraversable) yield return below;

        var right = rooms.GetOrCreateInstance((room.Position.X + 1, room.Position.Y));
        if (right.IsTraversable) yield return right;
    }

    public Room GetRoomAtLocation(int x, int y)
    {
        return rooms.GetOrCreateInstance((x, y));
    }
}