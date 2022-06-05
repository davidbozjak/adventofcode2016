using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

int payloadCounter = 0;
var discs = new InputProvider<Disc?>("Input.txt", GetDisc).Where(w => w != null).Cast<Disc>().ToList();

foreach (var disc in discs)
{
    disc.SetNeighbours(discs);
}

var targetData = discs.First(w => w.Position.Y == 0 && w.Position.X == 35); //real input end state
//var targetData = discs.First(w => w.Position.Y == 0 && w.Position.X == 2); //test end state
var targetNode = discs.First(w => w.Position.Y == 0 && w.Position.X == 0);

var endState = new EndStateDiscGrid(targetNode.Name, targetData.DataRegistry);
var startState = new DiscGrid(targetData, discs);

Console.WriteLine($"Part 1: {startState.GetMovesToNonEmptyDiscs().Count()}");

var heuristicTarget = new Point(0, 0);
int minHeuristic = int.MaxValue;

var path = AStarPathfinder.FindPath<DiscGrid>(startState, endState, GetHeuristicCost, state => state.GetMovedInvolvingNeighbouringCopies());

if (path == null) throw new Exception();

Console.WriteLine($"Part 2: {path.Count - 1}");
Console.ReadKey();

//reply position
var printer = new WorldPrinter();
foreach (var world in path)
{
    printer.Print(world);
    Console.ReadKey();
}

int GetHeuristicCost(DiscGrid discGrid)
{
    int manhattanDistanceSum = discGrid.DiscWithTargetData.Position.X * 5 + discGrid.DiscWithTargetData.Position.Y * 10;

    if (discGrid.EmptyDiscs.Count() != 1) throw new Exception();

    var emptyDisc = discGrid.EmptyDiscs.First();

    if (emptyDisc.Position.Y < 17)
    {
        manhattanDistanceSum += emptyDisc.Position.Y + emptyDisc.Position.X;
        manhattanDistanceSum += emptyDisc.Position.Distance(discGrid.DiscWithTargetData.Position) * 3;
    }
    else
    {
        manhattanDistanceSum += 200 + emptyDisc.Position.Y + Math.Max(0, (emptyDisc.Position.X - 2) * 10);
    }

    if (manhattanDistanceSum < minHeuristic)
    {
        minHeuristic = manhattanDistanceSum;

        Console.WriteLine($"{DateTime.Now.TimeOfDay}: {manhattanDistanceSum}");
    }

    return manhattanDistanceSum;
}

bool GetDisc(string? input, out Disc? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

    value = new Disc(parts[0], 
        ExtractNumberFromString(parts[1]), 
        ExtractNumberFromString(parts[2]),
        (payloadCounter++).ToString());

    return true;

    static int ExtractNumberFromString(string str)
    {
        Regex numRegex = new(@"\d+");
        return int.Parse(numRegex.Match(str).Value);
    }
}

class Disc : IWorldObject
{
    private readonly Cached<string> stateString;
    private readonly List<Disc> neighbours = new();

    public IEnumerable<Disc> Neighbours => this.neighbours;

    public string Name { get; }

    public string DataRegistry { get; }

    public Point Position { get; }

    public char CharRepresentation => this.Used == 0 ? '_' : 
        this.DataRegistry == "1050" ? this.Neighbours.Count(w => this.Used < w.Size).ToString()[0] : //1050 is a hack, quick and dirty way to visualize the target data
        this.Used > 90 ? 'B' : '.';

    public int Z => 0;

    public int Size { get; }

    public int Used { get; }

    public int Avaliable => this.Size - this.Used;

    public Disc(string name, int size, int used, string data)
    {
        this.stateString = new Cached<string>(this.BuildStateString);

        if (used > size) throw new Exception();

        this.Size = size;
        this.Used = used;
        this.DataRegistry = this.Used > 0 ? data : "";

        Regex numRegex = new(@"\d+");
        var numbersInName = numRegex.Matches(name).Select(w => int.Parse(w.Value)).ToArray();

        if (numbersInName.Length != 2) throw new Exception();

        this.Position = new Point(numbersInName[0], numbersInName[1]);
        this.Name = $"[{this.Position.X},{this.Position.Y}]";
    }
    public override string ToString() => this.stateString.Value;

    private string BuildStateString() => $"{this.Name}: {this.DataRegistry}";

    public void SetNeighbours(IEnumerable<Disc> potentialNeighbours)
    {
        this.neighbours.AddRange(potentialNeighbours.Where(w => this.Position.IsNeighbour(w.Position)));
    }
}

class DiscGrid : IWorld, INode, IEquatable<DiscGrid>
{
    private readonly Cached<string> stateString;

    private readonly List<Disc> allDiscs = new();
    private readonly List<Disc> emptyDiscs = new();

    public IEnumerable<IWorldObject> WorldObjects => this.allDiscs;

    public Disc DiscWithTargetData { get; }
    public IEnumerable<Disc> EmptyDiscs => this.emptyDiscs;

    public int Cost => 1;

    public DiscGrid(Disc discWithTargetData, IEnumerable<Disc> discs)
    {
        this.DiscWithTargetData = discWithTargetData;

        foreach (var disc in discs)
        {
            this.allDiscs.Add(disc);

            if (disc.Used == 0)
            {
                this.emptyDiscs.Add(disc);
            }
        }

        this.stateString = new Cached<string>(this.BuildStateString);
    }

    public IEnumerable<DiscGrid> GetMovesToNonEmptyDiscs()
    {
        for (int i = 0; i < allDiscs.Count; i++)
        {
            var discA = allDiscs[i];

            if (discA.Used <= 0) continue;

            for (int j = 0; j < allDiscs.Count; j++)
            {
                if (i == j) continue;

                var discB = allDiscs[j];

                if (discB.Avaliable >= discA.Used)
                {
                    yield return DataMove(discA, discB);
                }
            }
        }
    }

    public IEnumerable<DiscGrid> GetMovedInvolvingNeighbouringCopies()
    {
        if (this.allDiscs.Count == 0)
            throw new Exception("Not expecting this to be evaliated twice!");

        HashSet<DiscGrid> moves = new();

        foreach (var emptyDisc in this.emptyDiscs)
        {
            foreach (var neighbour in emptyDisc.Neighbours)
            {
                if (IsValidMove(emptyDisc, neighbour))
                {
                    moves.Add(DataMove(emptyDisc, neighbour));
                }
            }
        }

        return moves;

        static bool IsValidMove(Disc receivingDisc, Disc sendingDisc)
        {
            if (!receivingDisc.Position.IsNeighbour(sendingDisc.Position))
                return false;

            if (sendingDisc.Used == 0)
                return false;

            if (receivingDisc.Avaliable < sendingDisc.Used)
                return false;

            return true;
        }
    }
    private DiscGrid DataMove(Disc receivingDisc, Disc sendingDisc)
    {
        if (receivingDisc.Avaliable < sendingDisc.Used) throw new Exception();

        var newFullDisc = new Disc(receivingDisc.Name, receivingDisc.Size, receivingDisc.Used + sendingDisc.Used, 
            receivingDisc.DataRegistry + sendingDisc.DataRegistry);
        var newEmptyDisc = new Disc(sendingDisc.Name, sendingDisc.Size, 0, "");

        var discs = this.allDiscs.Except(new[] { receivingDisc, sendingDisc })
            .Append(newFullDisc)
            .Append(newEmptyDisc)
            .ToList();

        var newState = sendingDisc == this.DiscWithTargetData ?
            new DiscGrid(newFullDisc, discs) :
            new DiscGrid(this.DiscWithTargetData, discs);

        newFullDisc.SetNeighbours(discs);
        newEmptyDisc.SetNeighbours(discs);

        return newState;
    }

    public override string ToString() => this.stateString.Value;

    private string BuildStateString()
    {
        var builder = new StringBuilder();
        
        foreach (var disc in this.allDiscs.OrderBy(w => w.Name))
        {
            builder.AppendLine(disc.ToString());
        }

        return builder.ToString();
    }

    public override int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }

    public bool Equals(DiscGrid? other)
    {
        if (other == null) return false;
        
        if (other is EndStateDiscGrid endState)
        {
            return endState.Equals((object)this);
        }
        return this.GetHashCode() == other.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        return this.Equals(obj as DiscGrid);
    }
}

class EndStateDiscGrid : DiscGrid
{
    public string TargetName { get; }
    public string IncludedData { get; }

    public EndStateDiscGrid(string targetNodeName, string includedData)
        :base(null, Enumerable.Empty<Disc>())
    {
        this.TargetName = targetNodeName;
        this.IncludedData = includedData;
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj is EndStateDiscGrid)
        {
            return true; //for simplification assuming all end states are equivalent
        }
        else if (obj is DiscGrid discGrid)
        {
            var state = discGrid.ToString();

            if (state.Contains($"{this.TargetName}: {this.IncludedData}"))
                return true;
        }

        return false;
    }
}