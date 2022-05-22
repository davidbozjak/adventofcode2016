int startingNumberOfElfs = 3004953;

var elfs = Enumerable.Range(1, startingNumberOfElfs).Select(w => new Elf(w)).ToList();

for (int i = 0; i < startingNumberOfElfs - 1; i++)
    elfs[i].NextElf = elfs[i + 1];
elfs[startingNumberOfElfs - 1].NextElf = elfs[0];

var startingElf = elfs[0];
if (startingElf.Id != 1) throw new Exception();

while (startingElf != startingElf.NextElf)
{
    startingElf = startingElf.MakeStep();
}

Console.WriteLine($"Part 1: {startingElf.Id}");

class Elf
{
    private readonly List<int> gifts;

    public int CurrentGiftsCount => this.gifts.Count;

    public int Id { get; }

    public Elf NextElf { get; set; }

    public Elf (int id)
    {
        this.Id = id;
        this.gifts = new List<int>() { id };
    }

    public Elf MakeStep()
    {
        this.TakeGifts(this.NextElf);
        this.NextElf = this.NextElf.NextElf;
        return this.NextElf;
    }

    private void TakeGifts(Elf elf)
    {
        this.gifts.AddRange(elf.GiveGifts());
    }

    private IEnumerable<int> GiveGifts()
    {
        var giftsToGive = this.gifts;
        this.gifts.Clear();
        return giftsToGive;
    }
}