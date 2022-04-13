var addresses = new InputProvider<string>("Input.txt", GetString).ToList();

var tlsAddresses = addresses.Where(w => SupportsTLS(w));

Console.WriteLine($"Part 1: {tlsAddresses.Count()}");

static bool SupportsTLS(string address)
{
    var parts = address.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

    var hypernetSequences = Enumerable.Range(0, parts.Length).Where(w => w % 2 == 1).Select(w => parts[w]);
    var normalSequences = Enumerable.Range(0, parts.Length).Where(w => w % 2 == 0).Select(w => parts[w]);

    return normalSequences.Any(HasABBA) && hypernetSequences.All(w => !HasABBA(w));

    bool HasABBA(string str)
    {
        for (int i = 0; i < str.Length - 3; i++)
        {
            if ((str[i] == str[i + 3]) &&
                (str[i + 1] == str[i + 2]) &&
                (str[i] != str[i + 1]))
                return true;
        }

        return false;
    }
}

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}