using System.Text;

//string salt = "abc"; // debug string
string salt = "ngcjuoqr";

var generatedHashes = new List<string>();
var keys = new List<int>();

for (int index = 0; keys.Count < 64; index++)
{
    var c = FindFirstTripletInString(GetOrCreateHashN(index));

    if (c != null)
    {
        var strToConfirmKey = new string(Enumerable.Repeat((char)c, 5).ToArray());
        for (int i = 1; i <= 1000; i++)
        {
            var hash = GetOrCreateHashN(index + i);

            if (hash.Contains(strToConfirmKey))
            {
                keys.Add(index);
                break;
            }
        }
    }
}

Console.WriteLine($"Part 1: {keys[63]}");

string GetOrCreateHashN(int n)
{
    if (generatedHashes.Count <= n)
    {
        using (var md5 = System.Security.Cryptography.MD5.Create())
        {
            var existing = generatedHashes.Count;
            for (int i = 0; i < 2000; i++)
            {
                var input = Encoding.ASCII.GetBytes(salt + (existing + i).ToString());
                var hashbytes = md5.ComputeHash(input);

                var hexRepresentation = Convert.ToHexString(hashbytes);
                generatedHashes.Add(hexRepresentation);
            }
        }
    }

    return generatedHashes[n];
}

char? FindFirstTripletInString(string input)
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