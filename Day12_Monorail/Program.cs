var instructionStrings = new InputProvider<string?>("Input.txt", GetString).Cast<string>().ToList();

var computer = new Computer(instructionStrings);
computer.Run();

Console.WriteLine($"Part 1: {computer.GetRegisterValue("a")}");

computer = new Computer(instructionStrings);
computer.SetRegisterValue("c", 1);
computer.Run();

Console.WriteLine($"Part 2: {computer.GetRegisterValue("a")}");

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}