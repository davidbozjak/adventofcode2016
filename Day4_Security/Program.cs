using System.Text;

var roomHashes = new InputProvider<string?>("Input.txt", GetString).Where(w => w != null).Cast<string>().ToList();

var nameAndSectorId = roomHashes.Select(w => GetValidRoomHash(w));

Console.WriteLine($"Part 1: {nameAndSectorId.Sum(w => w.sectorId)}");

var validNameAndSectorId = nameAndSectorId
    .Where(w => w.sectorId > 0)
    .Select(w => $"{GetDecryptedName(w.encryptedName, w.sectorId)}, {w.sectorId}");

var namesAfterFiltering = validNameAndSectorId.Where(w => w.Contains("north"));

if (namesAfterFiltering.Count() != 1) throw new Exception();

Console.WriteLine($"Part 2: {namesAfterFiltering.First()}");

static (string encryptedName, int sectorId) GetValidRoomHash(string roomHash)
{
    var encryptedName = roomHash[0..roomHash.LastIndexOf("-")];
    var sectorId = int.Parse(roomHash[(roomHash.LastIndexOf("-")+1)..^7]);
    var checksum = roomHash[(roomHash.IndexOf("[") + 1)..^1];

    var letterFrequencies = encryptedName.GroupBy(w => w).ToDictionary(w => w.Key, w => w.Count());

    for (int i = 0; i < 4; i++)
    {
        if (!letterFrequencies.ContainsKey(checksum[i]) || !letterFrequencies.ContainsKey(checksum[i + 1]))
            return (encryptedName, 0);

        if (letterFrequencies[checksum[i]] < letterFrequencies[checksum[i + 1]])
        {
            return (encryptedName, 0);
        }
        else if (letterFrequencies[checksum[i]] == letterFrequencies[checksum[i + 1]])
        {
            if (checksum[i] > checksum[i + 1])
                return (encryptedName, 0);
        }
    }

    return (encryptedName, sectorId);
}

static string GetDecryptedName(string encryptedName, int sectorId)
{
    var builder = new StringBuilder();

    foreach (var c in encryptedName)
    {
        if (c == '-')
        {
            builder.Append(" ");
        }
        else
        {
            var newC = c;
            for (int i = 0; i < sectorId; i++)
                newC = GetNextChar(newC);

            builder.Append(newC);
        }
    }

    return builder.ToString();

    char GetNextChar(char ch)
    {
        if (ch == 'z') return 'a';
        else return (char)(ch + 1);
    }
}

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}