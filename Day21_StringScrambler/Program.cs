using System.Text.RegularExpressions;

var input = "abcdefgh";

var instructionList = new InputProvider<IInstruction?>("Input.txt", ParseInstructionFromInput).Where(w => w != null).Cast<IInstruction>().ToList();

var scrambledString = input;

instructionList.ForEach(w => scrambledString = w.Process(scrambledString));

Console.WriteLine($"Part 1: {scrambledString}");

scrambledString = "fbgdceah";
instructionList.Reverse();

instructionList.ForEach(w => scrambledString = w.ProcessReversed(scrambledString));

Console.WriteLine($"Part 2: {scrambledString}");

static bool ParseInstructionFromInput(string? input, out IInstruction? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"\d+");
    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    if (input.StartsWith("swap position"))
    {
        value = new SwapPositionInstruction(numbers[0], numbers[1]);
    }
    else if (input.StartsWith("swap letter"))
    {
        value = new SwapLetterInstruction(input[12], input[26]);
    }
    else if (input.StartsWith("rotate based on position of letter"))
    {
        value = new RotateRightBasedOnPositionInstruction(input[35]);
    }
    else if (input.StartsWith("rotate right"))
    {
        value = new RotateInstruction(numbers[0]);
    }
    else if (input.StartsWith("rotate left"))
    {
        value = new RotateInstruction(-numbers[0]);
    }
    else if (input.StartsWith("reverse positions"))
    {
        value = new ReversePositionsInstruction(numbers[0], numbers[1]);
    }
    else if (input.StartsWith("move position"))
    {
        value = new MovePositionsInstruction(numbers[0], numbers[1]);
    }
    else throw new Exception("Unknown instruction");

    return true;
}

