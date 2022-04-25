//string salt = "abc"; // debug string
string salt = "ngcjuoqr";

var normalKeys = Get64KeyIndexes(new HashProvider(salt, 0));
Console.WriteLine($"Part 1: {normalKeys.Last()}");

var stretchedKeys = Get64KeyIndexes(new HashProvider(salt, 2016));
Console.WriteLine($"Part 2: {stretchedKeys.Last()}");

static IList<int> Get64KeyIndexes(HashProvider hashProvider)
{
    var keys = new List<int>();

    for (int index = 0; keys.Count < 64; index++)
    {
        var c = FindFirstTripletInString(hashProvider.GetOrCreateHashN(index));

        if (c != null)
        {
            var strToConfirmKey = new string(Enumerable.Repeat((char)c, 5).ToArray());
            for (int i = 1; i <= 1000; i++)
            {
                var hash = hashProvider.GetOrCreateHashN(index + i);

                if (hash.Contains(strToConfirmKey))
                {
                    keys.Add(index);
                    break;
                }
            }
        }
    }

    return keys;
}

static char? FindFirstTripletInString(string input)
{
    int repeated = 1;

    for (int i = 1; i < input.Length; i++)
    {
        if (input[i] == input[i - 1])
            repeated++;
        else repeated = 1;

        if (repeated == 3)
        {
            return input[i];
        }
    }

    return null;
}