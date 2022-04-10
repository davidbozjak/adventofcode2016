using System.Text.RegularExpressions;

Regex numRegex = new(@"\d+");

var commaSeperatedSingleLineParser = new SingleLineStringInputParser<Instruction?>(GetInstruction, str => str.Split(", ", StringSplitOptions.RemoveEmptyEntries));
var instructions = new InputProvider<Instruction?>("Input.txt", commaSeperatedSingleLineParser.GetValue)
    .Where(w => w != null).Cast<Instruction>();

Heading heading = Heading.North;
int x = 0, y = 0;
List<(int x, int y)>? visitedLocations = new();

foreach (var instruction in instructions)
{
    heading += instruction.TurnDirection == Turn.Left ? -1 : +1;
    while ((int)heading > 4) heading -= 4;
    while ((int)heading < 1) heading += 4;

    for (int step = 0; step < instruction.NumberOfSteps; step++)
    {
        (x, y) = heading switch
        {
            Heading.North => (x, y - 1),
            Heading.South => (x, y + 1),
            Heading.East => (x + 1, y),
            Heading.West => (x - 1, y),
            _ => throw new Exception()
        };

        if (visitedLocations != null)
        {
            if (visitedLocations.Contains((x, y)))
            {
                Console.WriteLine($"Part 2: {TaxiDistance(x, y)}");
                visitedLocations = null;
            }

            visitedLocations?.Add((x, y));
        }
    }
}

Console.WriteLine($"Part 1: {TaxiDistance(x, y)}");

static int TaxiDistance(int x, int y) => Math.Abs(x) + Math.Abs(y);

static bool GetInstruction(string? input, out Instruction? value)
{
    value = null;

    if (input == null) return false;

    value = new Instruction(
        input.Contains('L') ? Turn.Left : Turn.Right,
        int.Parse(input[1..]));

    return true;
}

enum Heading { North = 1, East = 2, South = 3, West = 4}
enum Turn { Left, Right}
record Instruction (Turn TurnDirection, int NumberOfSteps);