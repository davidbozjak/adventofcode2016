//Part 1 Debug State
using System.Diagnostics;
using System.Text;

var elevator = new Item("E");

var lg = new Item("LG");
var lm = new Item("LM");
var hg = new Item("HG");
var hm = new Item("HM");

lm.SetForbiddenItemList(new[] { hg });
lm.ShieldItem = lg;

hm.SetForbiddenItemList(new[] { lg });
hm.ShieldItem = hg;

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

if (!IsStateValid(initialState)) throw new Exception();
if (!IsStateValid(desiredState)) throw new Exception();

var path = AStarPathfinder.FindPath(initialState, desiredState, w => 0, GetValidMoves);

Console.WriteLine($"Part 1: {path.Count - 1} steps");

IEnumerable<State> GetValidMoves(State state)
{
    var allMoves = GetAllMoves(state);

    var validMoves = allMoves.Where(IsStateValid).ToList();

    return validMoves;

    //return GetAllMoves(state).Where(IsStateValid);
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
        foreach (var item in floorWithoutElevator)
        {
            yield return MoveItemsFromFloorToFloor(elevatorFloor + 1, state, new[] { elevator, item });

            foreach (var secondItem in floorWithoutElevator)
            {
                if (item == secondItem) continue;

                yield return MoveItemsFromFloorToFloor(elevatorFloor + 1, state, new[] { elevator, item, secondItem });
            }
        }
    }

    //generate possible moves where items go down if not at bottom
    if (elevatorFloor > 0)
    {
        foreach (var item in floorWithoutElevator)
        {
            yield return MoveItemsFromFloorToFloor(elevatorFloor - 1, state, new[] { elevator, item });

            foreach (var secondItem in floorWithoutElevator)
            {
                if (item == secondItem) continue;

                yield return MoveItemsFromFloorToFloor(elevatorFloor - 1, state, new[] { elevator, item, secondItem });
            }
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
    foreach (var floor in state.Floors)
    {
        if (!IsFloorValid(floor))
            return false;
    }

    return true;

    bool IsFloorValid(IEnumerable<Item> floor)
    {
        foreach (var item in floor)
        {
            // In other words, if a chip is ever left in the same area as another RTG,
            // and it's not connected to its own RTG, the chip will be fried.
            if (floor.Any(w => item.ForbiddenItemList.Contains(w)))
            {
                if (!floor.Contains(item.ShieldItem))
                    return false;
            }
        }

        return true;
    }
}

[DebuggerDisplay("{Name}")]
class Item
{
    public string Name { get; }

    private readonly List<Item> forbiddenItemList = new();

    public IReadOnlyCollection<Item> ForbiddenItemList => this.forbiddenItemList.AsReadOnly();
    public Item ShieldItem { get; set; }


    public Item(string name)
    {
        this.Name = name;
    }

    public void SetForbiddenItemList(IEnumerable<Item> items)
    {
        if (this.forbiddenItemList.Count != 0) throw new Exception();

        this.forbiddenItemList.AddRange(items);
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