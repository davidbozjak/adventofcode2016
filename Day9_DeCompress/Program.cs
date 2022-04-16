string compressedString = new InputProvider<string?>("Input.txt", GetString).Where(w => w != null).Cast<string>().First();

Console.WriteLine($"Part 1: {GetDecompressedLength(compressedString, 0, compressedString.Length, false)}");
Console.WriteLine($"Part 2: {GetDecompressedLength(compressedString, 0, compressedString.Length, true)}");

static long GetDecompressedLength(string compressedString, int startIndex, int endIndex, bool enableRecursiveExpand)
{
    long decompressedLength = 0;

    for (int i = startIndex; i < endIndex; i++)
    {
        if (compressedString[i] == '(')
        {
            string noCharsString = string.Empty;
            for (i++; compressedString[i] != 'x'; i++)
            {
                noCharsString += compressedString[i];
            }

            string timesString = string.Empty;
            for (i++; compressedString[i] != ')'; i++)
            {
                timesString += compressedString[i];
            }

            int noChars = int.Parse(noCharsString);
            long expandedChars = noChars;
            
            if (enableRecursiveExpand)
            {
                expandedChars = GetDecompressedLength(compressedString, i + 1, i + 1 + noChars, true);
            }

            for (int times = int.Parse(timesString); times > 0; times--)
            {
                decompressedLength += expandedChars;
            }

            i += noChars;
        }
        else
        {
            decompressedLength++;
        }
    }

    return decompressedLength;
}

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}