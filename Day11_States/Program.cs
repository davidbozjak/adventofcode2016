//Part 1 Debug State
using System.Diagnostics;
using System.Text;

var elevator = new Item("E");

var lg = new Item("LG");
var lm = new Item("LM");
var hg = new Item("HG");
var hm = new Item("HM");

//Instructions:
//The first floor contains a hydrogen-compatible microchip and a lithium-compatible microchip.
//The second floor contains a hydrogen generator.
//The third floor contains a lithium generator.
//The fourth floor contains nothing relevant.

var initialState = new State(new[]
    {
        new[] {elevator, hm, lm},
        new[] {hg},
        new[] {lg},
        Array.Empty<Item>()
    });
var desiredState = new State(new[]
    {
        Array.Empty<Item>(),
        Array.Empty<Item>(),
        Array.Empty<Item>(),
        new [] { elevator, hg, hm, lm, lg }
    });

var path = AStarPathfinder.FindPath(initialState, desiredState, w => 0, GetValidMoves);

Console.WriteLine($"Part 1: {path.Count}");

IEnumerable<State> GetValidMoves(State state)
{
    return GetAllMoves(state).Where(IsStateValid);
}

IEnumerable<State> GetAllMoves(State state)
{
    //find floor with elevator

    int elevatorFloor = -1;

    for (int floor = 0; floor < state.Floors.Count; floor++)
    {
        if (state.Floors[floor].Contains(elevator))
        {
            elevatorFloor = floor;
            break;
        }
    }

    if (elevatorFloor == -1) throw new Exception();

    var floorWithoutElevator = state.Floors[elevatorFloor].Where(w => w != elevator).ToList();

    //generate possible moves where items go up if not on top
    if (elevatorFloor < 3)
    {
        //elevator moves up alone
        yield return MoveItemsFromFloorToFloor(elevatorFloor + 1, state, new[] { elevator });

        foreach (var permutation in floorWithoutElevator.PermuteList())
        {
            yield return MoveItemsFromFloorToFloor(elevatorFloor + 1, state, permutation.Append(elevator).ToList());
        }
    }

    //generate possible moves where items go down if not at bottom
    if (elevatorFloor > 1)
    {
        //elevator moves down alone
        yield return MoveItemsFromFloorToFloor(elevatorFloor - 1, state, new[] { elevator });

        foreach (var permutation in floorWithoutElevator.PermuteList())
        {
            yield return MoveItemsFromFloorToFloor(elevatorFloor - 1, state, permutation.Append(elevator));
        }
    }
}

State MoveItemsFromFloorToFloor(int newFloor, State originalState, IEnumerable<Item> itemsToMove)
{
    if (newFloor < 0 || newFloor > 3) throw new Exception();

    if (!itemsToMove.Any()) throw new Exception();

    List<Item>? floor1 = originalState.Floors[0].Where(w => !itemsToMove.Contains(w)).ToList();
    List<Item>? floor2 = originalState.Floors[1].Where(w => !itemsToMove.Contains(w)).ToList();
    List<Item>? floor3 = originalState.Floors[2].Where(w => !itemsToMove.Contains(w)).ToList();
    List<Item>? floor4 = originalState.Floors[3].Where(w => !itemsToMove.Contains(w)).ToList();

    var newFloors = new[]
    {
        floor1,
        floor2,
        floor3,
        floor4
    };

    newFloors[newFloor].AddRange(itemsToMove);

    var newState = new State(newFloors);

    if (newState.ContainedItemsCount != originalState.ContainedItemsCount)
        throw new Exception();

    if (!newState.Floors[newFloor].Contains(elevator)) throw new Exception();

    return newState;
}

static bool IsStateValid(State state)
{
    return true;
}

[DebuggerDisplay("{Name}")]
class Item
{
    public string Name { get; }

    private readonly List<Item> forbiddenItemList = new();
    private readonly List<Item> shildedItemList = new();

    public Item(string name)
    {
        this.Name = name;
    }
}

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