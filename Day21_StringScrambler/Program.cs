using System.Text;

//var wholeStringInput = new InputProvider<string>("Input.txt", GetString).ToList();

var input = "abcde";

var instructionList = new List<IInstruction>();

instructionList.Add(new SwapPositionInstruction(4, 0));
instructionList.Add(new SwapLetterInstruction('d', 'b'));
instructionList.Add(new ReversePositionsInstruction(0, 4));
instructionList.Add(new RotateInstruction(-1));
instructionList.Add(new MovePositionsInstruction(1, 4));
instructionList.Add(new MovePositionsInstruction(3, 0));
instructionList.Add(new RotateRightBasedOnPositionInstruction('b'));
instructionList.Add(new RotateRightBasedOnPositionInstruction('d'));

var scrambledString = input;

foreach (var instruction in instructionList)
{
    Console.Write($"{scrambledString} becomes ");
    scrambledString = instruction.Process(scrambledString);
    Console.WriteLine(scrambledString);
}

Console.WriteLine($"Part 1: {scrambledString}");

static bool GetString(string? input, out string? value)
{
    value = null;

    if (input == null) return false;

    value = input ?? string.Empty;

    return true;
}

interface IInstruction
{
    string Process(string input);
}

class SwapPositionInstruction : IInstruction
{
    private readonly int firstIndex;
    private readonly int secondIndex;

    public SwapPositionInstruction(int firstIndex, int secondIndex)
    {
        if (firstIndex == -1) throw new Exception();
        if (secondIndex == -1) throw new Exception();

        this.firstIndex = firstIndex;
        this.secondIndex = secondIndex;
    }

    public string Process(string input)
    {
        var builder = new StringBuilder(input);
        builder[firstIndex] = input[secondIndex];
        builder[secondIndex] = input[firstIndex];
        return builder.ToString();
    }
}

class SwapLetterInstruction : IInstruction
{
    private readonly char firstLetter;
    private readonly char secondLetter;

    public SwapLetterInstruction(char firstLetter, char secondLetter)
    {
        this.firstLetter = firstLetter;
        this.secondLetter = secondLetter;
    }

    public string Process(string input)
    {
        var firstIndex = input.IndexOf(firstLetter);
        var secondIndex = input.IndexOf(secondLetter);

        var swapPosition = new SwapPositionInstruction(firstIndex, secondIndex);
        return swapPosition.Process(input);
    }
}

class RotateInstruction : IInstruction
{
    private readonly int numberOfSteps;
    public RotateInstruction(int steps)
    {
        this.numberOfSteps = steps;
    }

    public string Process(string input)
    {
        var array = new char[input.Length];

        for(int i = 0; i < input.Length; i++)
        {
            array[wrapIndex(i + numberOfSteps)] = input[i];
        }

        return new string(array);

        int wrapIndex(int index)
        {
            while (index >= input.Length) index -= input.Length;
            while (index < 0) index += input.Length;

            return index;
        }
    }
}

class RotateRightBasedOnPositionInstruction : IInstruction
{
    private readonly char letter;

    public RotateRightBasedOnPositionInstruction(char letter)
    {
        this.letter = letter;
    }

    public string Process(string input)
    {
        int indexOfLetter = input.IndexOf(letter);
        if (indexOfLetter == -1) throw new Exception();

        int steps = 1 + indexOfLetter + (indexOfLetter >= 4 ? 1 : 0);
        
        var rotate = new RotateInstruction(steps);
        return rotate.Process(input);
    }
}

class ReversePositionsInstruction : IInstruction
{
    private readonly int lowIndex;
    private readonly int highIndex;

    public ReversePositionsInstruction(int firstIndex, int secondIndex)
    {
        if (firstIndex == -1) throw new Exception();
        if (secondIndex == -1) throw new Exception();

        this.lowIndex = Math.Min(firstIndex, secondIndex);
        this.highIndex = Math.Max(firstIndex, secondIndex);
    }

    public string Process(string input)
    {
        var builder = new StringBuilder(input);

        for (int i = this.lowIndex, j = this.highIndex; i <= this.highIndex; i++, j--)
        {
            builder[i] = input[j];
        }

        return builder.ToString();
    }
}

class MovePositionsInstruction : IInstruction
{
    private readonly int firstIndex;
    private readonly int secondIndex;

    public MovePositionsInstruction(int firstIndex, int secondIndex)
    {
        if (firstIndex == -1) throw new Exception();
        if (secondIndex == -1) throw new Exception();

        this.firstIndex = firstIndex;
        this.secondIndex = secondIndex;
    }

    public string Process(string input)
    {
        var list = input.ToList();

        var letter = input[firstIndex];
        list.Remove(letter);
        list.Insert(secondIndex, letter);

        return String.Join("", list);
    }
}