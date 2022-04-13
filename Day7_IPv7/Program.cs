var addresses = new InputProvider<string>("Input.txt", GetString).ToList();

Console.WriteLine($"Part 1: {addresses.Where(w => SupportsTLS(w)).Count()}");
Console.WriteLine($"Part 2: {addresses.Where(w => SupportsSSL(w)).Count()}");

static bool SupportsTLS(string address)
{
    var parts = address.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

    var normalSequences = Enumerable.Range(0, parts.Length).Where(w => w % 2 == 0).Select(w => parts[w]);
    var hypernetSequences = Enumerable.Range(0, parts.Length).Where(w => w % 2 == 1).Select(w => parts[w]);

    return normalSequences.Any(HasABBA) && hypernetSequences.All(w => !HasABBA(w));

    static bool HasABBA(string str)
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

static bool SupportsSSL(string address)
{
    var parts = address.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

    var normalSequences = Enumerable.Range(0, parts.Length).Where(w => w % 2 == 0).Select(w => parts[w]);
    var hypernetSequences = Enumerable.Range(0, parts.Length).Where(w => w % 2 == 1).Select(w => parts[w]);
    
    var abas = normalSequences.SelectMany(GetABAs);

    if (abas.Count() == 0) return false;

    return hypernetSequences
        .SelectMany(w => abas.Select(aba => new { Sequence = w, Aba = aba, IncludesBAB = w.Contains(GetBAB(aba)) }))
        .Any(w => w.IncludesBAB);

    static IEnumerable<string> GetABAs(string str)
    {
        List<string> abas = new();

        for (int i = 0; i < str.Length - 2; i++)
        {
            if ((str[i] == str[i + 2]) &&
                (str[i] != str[i + 1]))
                abas.Add(str.Substring(i, 3));
        }

        return abas;
    }

    static string GetBAB(string aba)
    {
        if (aba.Length != 3) throw new Exception();
        if (aba[0] != aba[2]) throw new Exception();
        if (aba[0] == aba[1]) throw new Exception();

        return new string(new[] { aba[1], aba[0], aba[1] });
    }
}

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}