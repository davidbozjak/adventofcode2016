string compressedString = new InputProvider<string?>("Input.txt", GetString).First();

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

            long noChars = int.Parse(noCharsString);
            //string strToRepeat = compressedString.Substring(i + 1, noChars);
            
            if (enableRecursiveExpand)
            {
                noChars = GetDecompressedLength(compressedString, i + 1, (int)(i + 1 + noChars), enableRecursiveExpand);
            }

            for (int times = int.Parse(timesString); times > 0; times--)
            {
                decompressedLength += noChars;
            }

            i += int.Parse(noCharsString);
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