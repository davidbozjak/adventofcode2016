var elevator = new Item("E");

//Debug State
//Instructions:
//The first floor contains a hydrogen-compatible microchip and a lithium-compatible microchip.
//The second floor contains a hydrogen generator.
//The third floor contains a lithium generator.
//The fourth floor contains nothing relevant.

//var lg = new Item("LG");
//var lm = new Item("LM");
//var hg = new Item("HG");
//var hm = new Item("HM");

//lm.SetForbiddenItemList(new[] { hg });
//lm.ShieldItem = lg;

//hm.SetForbiddenItemList(new[] { lg });
//hm.ShieldItem = hg;

//var initialState = new State(new[]
//    {
//        new[] { elevator, hm, lm },
//        new[] { hg },
//        new[] { lg },
//        Array.Empty<Item>()
//    });
//var desiredState = new State(new[]
//    {
//        Array.Empty<Item>(),
//        Array.Empty<Item>(),
//        Array.Empty<Item>(),
//        new [] { elevator, hg, hm, lm, lg }
//    });

//Real input
//Instructions
//The first floor contains a promethium generator (PG) and a promethium-compatible microchip (PM)
//The second floor contains a cobalt generator (CG), a curium generator (CUG), a ruthenium generator (RG), and a plutonium generator (PUG).
//The third floor contains a cobalt-compatible microchip (CM), a curium-compatible microchip (CUM), a ruthenium-compatible microchip (RM), and a plutonium-compatible microchip. (PUM)
//The fourth floor contains nothing relevant.

var pg = new Item("PG");
var pm = new Item("PM");
var cg = new Item("CG");
var cug = new Item("CUG");
var rg = new Item("RG");
var pug = new Item("PUG");
var cm = new Item("CM");
var cum = new Item("CUM");
var rm = new Item("RM");
var pum = new Item("PUM");

pm.SetForbiddenItemList(new[] { cg, cug, rg, pug });
pm.ShieldItem = pg;

cm.SetForbiddenItemList(new[] { pg, cug, rg, pug });
cm.ShieldItem = cg;

cum.SetForbiddenItemList(new[] { cg, pg, rg, pug });
cum.ShieldItem = cug;

rm.SetForbiddenItemList(new[] { cg, cug, pg, pug });
rm.ShieldItem = rg;

pum.SetForbiddenItemList(new[] { cg, cug, rg, pg });
pum.ShieldItem = pug;

var initialState = new State(new[]
    {
        new[] { elevator, pg, pm },
        new[] { cg, cug, rg, pug },
        new[] { cm, cum, rm, pum },
        Array.Empty<Item>()
    });
var desiredState = new State(new[]
    {
        Array.Empty<Item>(),
        Array.Empty<Item>(),
        Array.Empty<Item>(),
        new [] { elevator, pg, pm, cg, cug, rg, pug, cm, cum, rm, pum }
    });

if (!IsStateValid(initialState)) throw new Exception();
if (!IsStateValid(desiredState)) throw new Exception();
if (initialState.ContainedItemsCount != desiredState.ContainedItemsCount) throw new Exception();

var pathPart1 = AStarPathfinder.FindPath(initialState, desiredState, w => 0, GetValidMoves);

Console.WriteLine($"Part 1: {pathPart1.Count - 1} steps");

//Additional items for part 2:
var eg = new Item("EG");
var em = new Item("EM");
var dg = new Item("DG");
var dm = new Item("DM");

em.SetForbiddenItemList(new[] { cg, cug, rg, pg, pug, dg });
em.ShieldItem = eg;

dm.SetForbiddenItemList(new[] { cg, cug, rg, pg, pug, eg });
dm.ShieldItem = dg;

//include new elements in forbidden lists of original elements from part1
pm.SetForbiddenItemList(new[] { cg, cug, rg, pug, eg, dg });
cm.SetForbiddenItemList(new[] { pg, cug, rg, pug, eg, dg });
cum.SetForbiddenItemList(new[] { cg, pg, rg, pug, eg, dg });
rm.SetForbiddenItemList(new[] { cg, cug, pg, pug, eg, dg });
pum.SetForbiddenItemList(new[] { cg, cug, rg, pg, eg, dg });

//Include new elements in initial and desired states
initialState = new State(new[]
    {
        new[] { elevator, pg, pm, eg, em, dg, dm },
        new[] { cg, cug, rg, pug },
        new[] { cm, cum, rm, pum },
        Array.Empty<Item>()
    });
desiredState = new State(new[]
    {
        Array.Empty<Item>(),
        Array.Empty<Item>(),
        Array.Empty<Item>(),
        new [] { elevator, pg, pm, cg, cug, rg, pug, cm, cum, rm, pum, eg, em, dg, dm }
    });

var pathPart2 = AStarPathfinder.FindPath(initialState, desiredState, w => 0, GetValidMoves);

Console.WriteLine($"Part 2: {pathPart2.Count - 1} steps");

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
