using System.Diagnostics;

int startingNumberOfElfs = 3004953;

List<Elf> elfs = GetNElfsConnectedInACircle(startingNumberOfElfs);

var activeElf = elfs[0];
if (activeElf.Id != 1) throw new Exception();

while (activeElf != activeElf.NextElf)
{
    activeElf.TakeGifts(activeElf.NextElf);
    activeElf.NextElf = activeElf.NextElf.NextElf;
    activeElf = activeElf.NextElf;
}

Console.WriteLine($"Part 1: {activeElf.Id}");

elfs = GetNElfsConnectedInACircle(startingNumberOfElfs);
int remaining = elfs.Count;

activeElf = elfs[0];
if (activeElf.Id != 1) throw new Exception();
var accrossElf = elfs[elfs.Count / 2];

while (activeElf != activeElf.NextElf)
{
    activeElf.TakeGifts(accrossElf);

    accrossElf.PreviousElf.NextElf = accrossElf.NextElf;
    accrossElf.NextElf.PreviousElf = accrossElf.PreviousElf;

    activeElf = activeElf.NextElf;
    accrossElf = remaining % 2 == 1 ? accrossElf.NextElf.NextElf : accrossElf.NextElf;
    remaining--;
}

Console.WriteLine($"Part 2: {activeElf.Id}");

static List<Elf> GetNElfsConnectedInACircle(int numberOfElfs)
{
    var elfs = Enumerable.Range(1, numberOfElfs).Select(w => new Elf(w)).ToList();
    int accrossIndex = elfs.Count / 2;

    for (int i = 0; i < numberOfElfs; i++)
    {
        elfs[i].NextElf = elfs[GetWrappedIndex(i + 1)];
        elfs[i].PreviousElf = elfs[GetWrappedIndex(i - 1)];
    }

    return elfs;

    int GetWrappedIndex(int index)
    {
        while (index >= numberOfElfs) index -= numberOfElfs;
        while (index < 0) index += numberOfElfs;
        return index;
    }
}

[DebuggerDisplay("[{Id}] Next: {NextElf?.Id ?? null} Previous: {PreviousElf?.Id ?? null} Accross: {AccrossElf?.Id ?? null}")]
class Elf
{
    private readonly List<int> gifts;

    public int CurrentGiftsCount => this.gifts.Count;

    public int Id { get; }

    public Elf NextElf { get; set; }

    public Elf PreviousElf { get; set; }

    public Elf(int id)
    {
        this.Id = id;
        this.gifts = new List<int>() { id };
    }

    public void TakeGifts(Elf elf)
    {
        this.gifts.AddRange(elf.GiveGifts());
    }

    public IEnumerable<int> GiveGifts()
    {
        var giftsToGive = this.gifts.ToList();
        this.gifts.Clear();
        return giftsToGive;
    }
}