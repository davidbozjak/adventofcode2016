var instructionStrings = new InputProvider<string?>("Input.txt", GetString).Cast<string>().ToList();

var computer = new Computer(instructionStrings);

computer.SetRegisterValue("a", 7);

computer.Run();

Console.WriteLine($"Part 1: {computer.GetRegisterValue("a")}");

// for part 2 deconstructing the code

long a = 12;

// bulk of the input code calculates the factorial of the input
for (int i = (int)a - 1; i > 1; i--)
    a *= i;

// lines 20 - 26 add 84*89 to a *after they have been toggled*
a += 84 * 89;

Console.WriteLine($"Part 2: {a}");

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}