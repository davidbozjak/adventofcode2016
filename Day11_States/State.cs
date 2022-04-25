using System.Diagnostics;
using System.Text;

[DebuggerDisplay("{ToString()}")]
class State : INode, IEquatable<State>
{
    private readonly Cached<string> stateStringRepresentation;
    private readonly List<List<Item>> items;

    public int Cost => 1;

    public int ContainedItemsCount => items.Sum(w => w.Count);

    public IReadOnlyList<IReadOnlyCollection<Item>> Floors => this.items.Select(w => w.AsReadOnly()).ToList().AsReadOnly();

    public State(IEnumerable<Item>[] items)
    {
        this.stateStringRepresentation = new Cached<string>(GenerateStringRepresentation);
        this.items = items.Select(w => w.ToList()).ToList();

        if (this.items.Count != 4) throw new Exception();
    }

    private string GenerateStringRepresentation()
    {
        var builder = new StringBuilder();

        foreach (var floor in items)
        {
            builder.AppendLine($"<<{string.Join(", ", floor.Select(w => w.Name).OrderBy(w => w))}>>");
        }

        return builder.ToString();
    }

    public override string ToString()
    {
        return this.stateStringRepresentation.Value;
    }

    public bool Equals(State? other)
    {
        if (other == null) return false;

        return this.GetHashCode() == other.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (obj is State state)
        {
            return this.Equals(state);
        }
        else return false;
    }

    public override int GetHashCode()
    {
        return this.ToString().GetHashCode();
    }
}