using System.Text.RegularExpressions;

var instructions = new InputProvider<Instruction?>("Input.txt", GetInstruction).Where(w => w != null).Cast<Instruction>().ToList();

int displayWidth = 50;
int displayHeight = 6;

var displayBits = new int[displayWidth, displayHeight];

foreach (var instruction in instructions)
{
    if (instruction is FillRectInstruction fillRectInstruction)
    {
        for (int x = 0; x < fillRectInstruction.Width; x++)
        {
            for (int y = 0; y < fillRectInstruction.Height; y++)
            {
                displayBits[x, y] = 1;
            }
        }
    }
    else if (instruction is RotateColumnInstruction rotateColumnInstruction)
    {
        int[] newColumn = new int[displayHeight];
        for (int y = 0; y < displayHeight; y++)
        {
            int newY = y + rotateColumnInstruction.Steps;
            while (newY >= displayHeight) newY -= displayHeight;
            newColumn[newY] = displayBits[rotateColumnInstruction.Column, y];
        }
        for (int y = 0; y < displayHeight; y++)
        {
            displayBits[rotateColumnInstruction.Column, y] = newColumn[y];
        }
    }
    else if (instruction is RotateRowInstruction rotateRowInstruction)
    {
        int[] newRow = new int[displayWidth];
        for (int x = 0; x < displayWidth; x++)
        {
            int newX = x + rotateRowInstruction.Steps;
            while (newX >= displayWidth) newX -= displayWidth;
            newRow[newX] = displayBits[x, rotateRowInstruction.Row];
        }
        for (int x = 0; x < displayWidth; x++)
        {
            displayBits[x, rotateRowInstruction.Row] = newRow[x];
        }
    }
    else throw new Exception();
}

int totalOne = 0;
for (int y = 0; y < displayHeight; y++) 
{
    for (int x = 0; x < displayWidth; x++)
    {
        if (displayBits[x, y] == 1)
        {
            totalOne++;
            Console.Write("#");
        }
        else Console.Write(" ");
    }
    Console.WriteLine();
}

Console.WriteLine($"Part 1: {totalOne}");

static bool GetInstruction(string? input, out Instruction? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"\d+");
    var numbers = numRegex.Matches(input).Select(w => int.Parse(w.Value)).ToArray();

    if (input.Contains("rect"))
    {
        value = new FillRectInstruction()
        {
            Width = numbers[0],
            Height = numbers[1]
        };
    }
    else if (input.Contains("row"))
    {
        value = new RotateRowInstruction()
        {
            Row = numbers[0],
            Steps = numbers[1]
        };
    }
    else if (input.Contains("column"))
    {
        value = new RotateColumnInstruction()
        {
            Column = numbers[0],
            Steps = numbers[1]
        };
    }
    else throw new Exception();

    return true;
}

class Instruction
{

}

class FillRectInstruction : Instruction
{
    public int Height { get; init;  }
    
    public int Width { get; init; }
}

class RotateColumnInstruction : Instruction
{
    public int Column { get; init; }
     
    public int Steps { get; init; }
}

class RotateRowInstruction : Instruction
{
    public int Row { get; init; }
     
    public int Steps { get; init; }
}