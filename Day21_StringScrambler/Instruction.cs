using System.Text;

interface IInstruction
{
    string Process(string input);

    string ProcessReversed(string input);
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

    public string ProcessReversed(string input) => Process(input);
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

    public string ProcessReversed(string input) => Process(input);
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

        for (int i = 0; i < input.Length; i++)
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

    public string ProcessReversed(string input)
    {
        var reverseRotation = new RotateInstruction(-numberOfSteps);
        return reverseRotation.Process(input);
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

    public string ProcessReversed(string input)
    {
        int indexOfLetter = input.IndexOf(letter);
        if (indexOfLetter == -1) throw new Exception();

        if (input.Length != 8) throw new Exception("The hack only works for input strings with a length of 8");

        int steps = indexOfLetter switch
        {
            0 => -1,
            1 => -1,
            2 => -6,
            3 => -2,
            4 => -7,
            5 => -3,
            6 => 0,
            7 => -4,
            _ => throw new Exception()
        };

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

    public string ProcessReversed(string input) => Process(input);
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

        return string.Join("", list);
    }

    public string ProcessReversed(string input)
    {
        var reverseMove = new MovePositionsInstruction(this.secondIndex, this.firstIndex);
        return reverseMove.Process(input);
    }
}