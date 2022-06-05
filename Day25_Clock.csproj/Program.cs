var instructionStrings = new InputProvider<string?>("Input.txt", GetString).Cast<string>().ToList();

int numberOfRequiredCorrects = 10;
int numberOfCorrects = 0;
long expectedValue = 0;

for (long a = 1; ; a++, expectedValue = 0, numberOfCorrects = 0)
{
    var computer = new Computer(instructionStrings, GetOutputAndContinue);
    computer.SetRegisterValue("a", a);

    computer.Run();

    if (numberOfCorrects == numberOfRequiredCorrects)
    {
        Console.WriteLine($"Part 1: {a}");
        break;
    }
}

bool GetOutputAndContinue(long value)
{
    if (value == expectedValue)
    {
        expectedValue = (expectedValue + 1) % 2;
        numberOfCorrects++;

        if (numberOfCorrects == numberOfRequiredCorrects)
        {
            return false;
        }

        return true;
    }
    else
    {
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