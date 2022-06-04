using System.Text.RegularExpressions;

var discs = new InputProvider<Disk?>("Input.txt", GetDisk).Where(w => w != null).Cast<Disk>().ToList();

List<(Disk, Disk)> discPairs = new();

for (int i = 0; i < discs.Count; i++)
{
    var diskA = discs[i];

    if (diskA.Used <= 0) continue;

    for (int j = 0; j < discs.Count; j++)
    {
        if (i == j) continue;

        var diskB = discs[j];

        if (diskB.Avail >= diskA.Used)
        {
            discPairs.Add((diskA, diskB));
        }
    }
}

Console.WriteLine($"Part 1: {discPairs.Count}");

static bool GetDisk(string? input, out Disk? value)
{
    value = null;

    if (input == null) return false;

    var parts = input.Split(" ", StringSplitOptions.RemoveEmptyEntries);

    value = new Disk(parts[0], 
        ExtractNumberFromString(parts[1]), 
        ExtractNumberFromString(parts[2]), 
        ExtractNumberFromString(parts[3]), 
        ExtractNumberFromString(parts[4]));

    return true;

    int ExtractNumberFromString(string str)
    {
        Regex numRegex = new(@"\d+");
        return int.Parse(numRegex.Match(str).Value);
    }
}

record Disk(string Name, int Size, int Used, int Avail, int PercentageUsed);