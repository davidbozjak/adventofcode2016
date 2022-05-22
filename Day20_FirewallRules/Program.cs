using System.Text.RegularExpressions;

var blockedranges = new InputProvider<BlockedRange?>("Input.txt", GetBlockedRanges).Where(w => w != null).Cast<BlockedRange>().ToList();

var sorted = blockedranges.OrderBy(w => w.Start);

long lowestAllowed = 0;

bool foundAny = true;

while (foundAny)
{
    foundAny = false;
    foreach (var blockedRange in sorted)
    {
        if ((blockedRange.Start <= lowestAllowed) && (blockedRange.End >= lowestAllowed))
        {
            lowestAllowed = blockedRange.End + 1;
            foundAny = true;
            break;
        }
    }
}

Console.WriteLine($"Part 1: {lowestAllowed}");

var nonOverlappingRanges = blockedranges.OrderBy(w => w.Start).ToList();

foundAny = true;

while (foundAny)
{
    foundAny = false;

    for (int i = 0; i < nonOverlappingRanges.Count; i++)
    {
        for (int j = 0; j < nonOverlappingRanges.Count; j++)
        {
            if (i == j) continue;

            if (IsBetween(nonOverlappingRanges[j].Start, nonOverlappingRanges[i].Start, nonOverlappingRanges[i].End) ||
                IsBetween(nonOverlappingRanges[j].End, nonOverlappingRanges[i].Start, nonOverlappingRanges[i].End))
            {
                nonOverlappingRanges[i].Start = Math.Min(nonOverlappingRanges[i].Start, nonOverlappingRanges[j].Start);
                nonOverlappingRanges[i].End = Math.Max(nonOverlappingRanges[i].End, nonOverlappingRanges[j].End);
                
                foundAny = true;
                nonOverlappingRanges.Remove(nonOverlappingRanges[j]);
                i = j = nonOverlappingRanges.Count;
                break;
            }
        }
    }
}

Console.WriteLine($"Part 2: {(long)UInt32.MaxValue + 1 - nonOverlappingRanges.Sum(w => w.CountInBetween)}");

static bool IsBetween(long num, long min, long max)
{
    return num >= min && num <= max;
}

static bool GetBlockedRanges(string? input, out BlockedRange? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"\d+");

    var numbers = numRegex.Matches(input).Select(w => long.Parse(w.Value)).ToArray();

    if (numbers.Length != 2) throw new Exception();

    value = new BlockedRange(numbers[0], numbers[1]);

    return true;
}

class BlockedRange
{
    public long Start { get; set; }
     
    public long End { get; set; }
     
    public long CountInBetween => this.End - this.Start + 1;

    public BlockedRange(long start, long end)
    {
        this.Start = start;
        this.End = end;
    }
}