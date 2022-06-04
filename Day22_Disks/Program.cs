using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;

var discs = new InputProvider<Disc?>("Input.txt", GetDisc).Where(w => w != null).Cast<Disc>().ToList();

var startState = new DiscGrid(discs);

//Console.WriteLine($"Part 1: {startState.GetMovesToNonEmptyDiscs().Count()}");

var endState = new EndStateDiscGrid("node-x0-y0", "/dev/grid/node-x35-y0");  //real input end state
//var endState = new EndStateDiscGrid("node-x0-y0", "/dev/grid/node-x2-y0"); //test end state

var path = AStarPathfinder.FindPath<DiscGrid>(startState, endState, GetHeuristicCost, state => state.GetMovedInvolvingNeighbouringCopies());

if (path == null) throw new Exception();

Console.WriteLine($"Part 2: {path.Count - 1}");
Console.ReadKey();

int GetHeuristicCost(DiscGrid discGrid)
{
    var targetDataLocation = discGrid.Discs.Where(w => w.DataRegistry.Contains(endState.IncludedData)).FirstOrDefault();

    if (targetDataLocation == null) throw new Exception("Sought after data dissapeared");

    return targetDataLocation.Position.X + targetDataLocation.Position.Y;
}

static bool GetDisc(string? input, out Disc? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

    value = new Disc(parts[0], 
        ExtractNumberFromString(parts[1]), 
        ExtractNumberFromString(parts[2]),
        parts[0]);

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

    public string Name { get; }

    public string DataRegistry { get; }

    public Point Position { get; }

    public char CharRepresentation => this.Used > 0 ? '.' : '_';

    public int Z => 0;

    public int Size { get; }

    public int Used { get; }

    public int Avaliable => this.Size - this.Used;

    public Disc(string name, int size, int used, string data)
    {
        this.stateString = new Cached<string>(this.BuildStateString);

        this.Name = name;
        
        if (used > size) throw new Exception();
        
        this.Size = size;
        this.Used = used;
        this.DataRegistry = this.Used > 0 ? data : "";

        Regex numRegex = new(@"\d+");
        var numbersInName = numRegex.Matches(name).Select(w => int.Parse(w.Value)).ToArray();

        if (numbersInName.Length != 2) throw new Exception();

        this.Position = new Point(numbersInName[0], numbersInName[1]);
    }

    public override string ToString() => this.stateString.Value;

    private string BuildStateString() => $"{this.Name}: {this.DataRegistry}";
}

class DiscGrid : IWorld, INode, IEquatable<DiscGrid>
{
    private readonly Cached<List<Disc>> discs;
    private readonly Cached<string> stateString;

    public IEnumerable<Disc> Discs => this.discs.Value;
    public IEnumerable<IWorldObject> WorldObjects => this.discs.Value;

    public int Cost => 1;

    public DiscGrid(IEnumerable<Disc> discs)
    {
        this.discs = new Cached<List<Disc>>(() => discs.ToList());
        this.stateString = new Cached<string>(this.BuildStateString);
    }

    public IEnumerable<DiscGrid> GetMovesToNonEmptyDiscs()
    {
        for (int i = 0; i < discs.Value.Count; i++)
        {
            var discA = discs.Value[i];

            if (discA.Used <= 0) continue;

            for (int j = 0; j < discs.Value.Count; j++)
            {
                if (i == j) continue;

                var discB = discs.Value[j];

                if (discB.Avaliable >= discA.Used)
                {
                    yield return DataMove(discA, discB);
                }
            }
        }
    }

    public IEnumerable<DiscGrid> GetMovedInvolvingNeighbouringCopies()
    {
        for (int i = 0; i < discs.Value.Count; i++)
        {
            var discA = discs.Value[i];

            for (int j = 0; j < discs.Value.Count; j++)
            {
                if (i == j) continue;

                var discB = discs.Value[j];

                if (discB.Used <= 0) continue; //Don't include moves copying empty discs onto other discs

                if (discA.Avaliable >= discB.Used)
                {
                    if (!discA.Position.IsNeighbour(discB.Position))
                        continue;

                    yield return DataMove(discA, discB);
                }
            }
        }
    }
    private DiscGrid DataMove(Disc receivingDisc, Disc sendingDisc)
    {
        if (receivingDisc.Avaliable < sendingDisc.Used) throw new Exception();

        var newFullDisc = new Disc(receivingDisc.Name, receivingDisc.Size, receivingDisc.Used + sendingDisc.Used, 
            receivingDisc.DataRegistry + sendingDisc.DataRegistry);
        var newEmptyDisc = new Disc(sendingDisc.Name, sendingDisc.Size, 0, "");

        var discs = this.discs.Value.Except(new[] { receivingDisc, sendingDisc }).Append(newFullDisc).Append(newEmptyDisc);

        return new DiscGrid(discs);
    }

    public override string ToString() => this.stateString.Value;

    private string BuildStateString()
    {
        var builder = new StringBuilder();
        
        foreach (var disc in this.discs.Value.OrderBy(w => w.Name))
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
        :base(Enumerable.Empty<Disc>())
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