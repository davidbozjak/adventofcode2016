using System.Drawing;
using System.Text;

ShiftingCell.RootForHash  = "dmypynyp";

var maze = new Maze(4, 4);
var initialState = (ShiftingCell)maze.GetCellOrNull(0, 0, "");

var path = AStarPathfinder.FindPath(initialState, maze.EndCell, _ => 0, c => c.GetNeighbours());

Console.WriteLine($"Part 1: {path.Last().Path}");

class ShiftingCell : INode, IWorldObject, IEquatable<ShiftingCell>
{
    public static string RootForHash { get; set; }
    public int Cost => 1;

    public Point Position { get; }

    public char CharRepresentation => ' ';

    public int Z => 1;

    public string Path { get; private set; }

    private readonly Maze Maze;
    
    private readonly Cached<IEnumerable<ShiftingCell>> cachedNeighbourCells;

    public ShiftingCell(int x, int y, string path, Maze maze)
    {
        this.Position = new Point(x, y);
        this.Path = path;
        this.Maze = maze;
        this.cachedNeighbourCells = new Cached<IEnumerable<ShiftingCell>>(this.GenerateNeighbours);
    }

    public IEnumerable<ShiftingCell> GetNeighbours()
    {
        return this.cachedNeighbourCells.Value;
    }

    public void OverridePath(string path)
    {
        this.Path = path;
    }

    private IEnumerable<ShiftingCell> GenerateNeighbours()
    {
        var list = new List<ShiftingCell>();

        (bool up, bool down, bool left, bool right) = GenerateDoorStatus();

        if (up) list.AddIfNotNull(this.Maze.GetCellOrNull(this.Position.X, this.Position.Y - 1, this.Path + "U"));
        if (down) list.AddIfNotNull(this.Maze.GetCellOrNull(this.Position.X, this.Position.Y + 1, this.Path + "D"));
        if (left) list.AddIfNotNull(this.Maze.GetCellOrNull(this.Position.X - 1, this.Position.Y, this.Path + "L"));
        if (right) list.AddIfNotNull(this.Maze.GetCellOrNull(this.Position.X + 1, this.Position.Y, this.Path + "R"));

        return list;
    }

    private (bool up, bool down, bool left, bool right) GenerateDoorStatus()
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            var bytes = Encoding.ASCII.GetBytes(RootForHash + this.Path);
            var hashbytes = md5.ComputeHash(bytes);

            var hash = Convert.ToHexString(hashbytes).ToLower();

            return (IsOpen(hash[0]), IsOpen(hash[1]), IsOpen(hash[2]), IsOpen(hash[3]));
        }

        static bool IsOpen(char c)
        {
            return new[] { 'b', 'c', 'd', 'e', 'f' }.Contains(c);
        }
    }

    public bool Equals(ShiftingCell? other)
    {
        if (other == null) return false;
        else return this == other;
    }
}

class Maze : IWorld
{
    private readonly UniqueFactory<(int, int, string), ShiftingCell> CellFactory;

    public IEnumerable<IWorldObject> WorldObjects => this.CellFactory.AllCreatedInstances.Cast<IWorldObject>();

    public int Height { get; }

    public int Width { get; }

    public Maze(int width, int height)
    {
        this.Width = width;
        this.Height = height;

        this.CellFactory = new UniqueFactory<(int x, int y, string path), ShiftingCell>((p) => new ShiftingCell(p.x, p.y, p.path, this));
        
        this.EndCell = new ShiftingCell(this.Width - 1, this.Height - 1, "", this);
    }

    public ShiftingCell EndCell { get; }

    public ShiftingCell? GetCellOrNull(int x, int y, string path)
    {
        if (x == this.EndCell.Position.X && y == this.EndCell.Position.Y)
        {
            this.EndCell.OverridePath(path);
            return this.EndCell;
        }

        if (x < 0) return null;
        if (x >= this.Width) return null;

        if (y < 0) return null;
        if (y >= this.Height) return null;

        return this.CellFactory.GetOrCreateInstance((x, y, path));
    }
}