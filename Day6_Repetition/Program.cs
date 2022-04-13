using System.Text;

var messages = new InputProvider<string?>("Input.txt", GetString).Where(w => w != null).Cast<string>().ToList();

var messageLength = messages[0].Length;
var mostFrequentKeyMessageBuilder = new StringBuilder();
var leastFrequentKeyMessageBuilder = new StringBuilder();

for (int i = 0; i < messageLength; i++)
{
    var letters = messages.Select(w => w[i]);
    var frequencies = letters.GroupBy(w => w).ToDictionary(w => w.Key, w => w.Count());
    
    var mostFrequent = frequencies.OrderByDescending(w => w.Value).Select(w => w.Key).First();
    mostFrequentKeyMessageBuilder.Append(mostFrequent);
    
    var leastFrequent = frequencies.OrderBy(w => w.Value).Select(w => w.Key).First();
    leastFrequentKeyMessageBuilder.Append(leastFrequent);
}

Console.WriteLine($"Part 1: {mostFrequentKeyMessageBuilder}");
Console.WriteLine($"Part 2: {leastFrequentKeyMessageBuilder}");

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}