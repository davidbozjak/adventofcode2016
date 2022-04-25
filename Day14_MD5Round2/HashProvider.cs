using System.Text;

class HashProvider
{
    private readonly List<string> generatedHashes = new();
    private readonly string salt;
    private readonly int numberOfHashIterations;

    public HashProvider(string salt, int numberOfHashIterations = 0)
    {
        this.salt = salt;
        this.numberOfHashIterations = numberOfHashIterations;
    }

    public string GetOrCreateHashN(int n)
    {
        if (generatedHashes.Count <= n)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var existing = generatedHashes.Count;
                for (int i = 0; i < 2000; i++)
                {
                    string input = salt + (existing + i).ToString();

                    for (int j = 0; j <= numberOfHashIterations; j++)
                    {
                        var bytes = Encoding.ASCII.GetBytes(input);
                        var hashbytes = md5.ComputeHash(bytes);

                        input = Convert.ToHexString(hashbytes).ToLower();
                    }

                    generatedHashes.Add(input);
                }
            }
        }

        return generatedHashes[n];
    }
}