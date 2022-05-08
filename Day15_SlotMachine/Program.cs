using System.Text.RegularExpressions;

var discs = new InputProvider<RotatingDisc?>("Input.txt", GetRotatingDisc).Where(w => w != null).Cast<RotatingDisc>().ToList();

Console.WriteLine($"Part 1: Delay for {GetSmallestTimeDelay(discs)}");

discs.ForEach(w => w.Reset());
discs.Add(new RotatingDisc(11, 0));

Console.WriteLine($"Part 2: Delay for {GetSmallestTimeDelay(discs)}");

static long GetSmallestTimeDelay(List<RotatingDisc> discs)
{
    long timeDelay = 0;
    long cycleMultiplyer = 1;

    for (int i = 0; i < discs.Count; i++)
    {
        var currentDisc = discs[i];

        while (currentDisc.StepsToZero != ((i + 1) % currentDisc.PositionsCount))
        {
            discs.ForEach(w => w.AdvanceSteps(cycleMultiplyer));
            timeDelay += cycleMultiplyer;
        }

        cycleMultiplyer *= currentDisc.PositionsCount;
    }

    return timeDelay;
}

static bool GetRotatingDisc(string? input, out RotatingDisc? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"\d+");

    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    if (numbers.Length != 4) throw new Exception();

    value = new RotatingDisc(numbers[1], numbers[3]);

    return true;
}

class RotatingDisc
{
    private readonly int initialPosition;
    public int Position { get; private set; }

    public int PositionsCount { get; }

    public RotatingDisc(int count, int initialPosition)
    {
        this.PositionsCount = count;
        this.initialPosition = initialPosition;
        this.Position = initialPosition;
    }

    public void Reset()
    {
        this.Position = this.initialPosition;
    }

    public void AdvanceSteps(long steps)
    {
        steps = steps % this.PositionsCount;

        this.Position += (int)steps;

        while (this.Position >= this.PositionsCount) this.Position -= this.PositionsCount;
    }

    public int StepsToZero => this.PositionsCount - this.Position;
}