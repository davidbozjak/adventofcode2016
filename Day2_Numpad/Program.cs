using System.Text;

var instructions = new InputProvider<Instruction?>("Input.txt", GetInstructions)
    .Where(w => w != null).Cast<Instruction>()
    .ToList();

Dictionary<(int x, int y), char> keyPad = new()
{
    { (0, 0), '1' },
    { (1, 0), '2' },
    { (2, 0), '3' },
    { (0, 1), '4' },
    { (1, 1), '5' },
    { (2, 1), '6' },
    { (0, 2), '7' },
    { (1, 2), '8' },
    { (2, 2), '9' },
};

int x = 1, y = 1;
StringBuilder codeWIP = new();

foreach (var instruction in instructions)
{
    foreach (var step in instruction.Steps)
    {
        (x, y) = step switch
        {
            Direction.Up => (x, y - 1),
            Direction.Down => (x, y + 1),
            Direction.Left => (x - 1, y),
            Direction.Right => (x + 1, y),
            _ => throw new Exception()
        };
        x = Math.Max(0, Math.Min(2, x));
        y = Math.Max(0, Math.Min(2, y));
    }

    codeWIP.Append(keyPad[(x, y)]);
}

Console.WriteLine($"Part 1: {codeWIP}");

static bool GetInstructions(string? input, out Instruction? value)
{
    value = null;

    if (input == null) return false;

    value = new Instruction(
        input.Select(w => w switch
        {
            'L' => Direction.Left,
            'U' => Direction.Up,
            'D' => Direction.Down,
            'R' => Direction.Right,
            _ => throw new Exception()
        }));

    return true;
}

enum Direction { Up, Left, Right, Down}
record Instruction(IEnumerable<Direction> Steps);