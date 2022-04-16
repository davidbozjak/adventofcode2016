using System.Text;

string compressedString = new InputProvider<string?>("Input.txt", GetString).First();

StringBuilder outputBuilder = new StringBuilder();

for (int i = 0; i < compressedString.Length; i++)
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
        string strToRepeat = compressedString.Substring(i + 1, noChars);
        i += noChars;

        for (int times = int.Parse(timesString); times > 0; times--)
        {
            outputBuilder.Append(strToRepeat);
        }
    }
    else
    { 
        outputBuilder.Append(compressedString[i]); 
    }
}

Console.WriteLine($"Part 1: {outputBuilder.Length}");
Console.WriteLine(outputBuilder);

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}