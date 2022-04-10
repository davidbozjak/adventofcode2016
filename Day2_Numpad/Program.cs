using System.Text;

var instructions = new InputProvider<Instruction?>("Input.txt", GetInstructions)
    .Where(w => w != null).Cast<Instruction>()
    .ToList();

Dictionary<(int x, int y), char> part1KeyPad = new()
{
    { (0, 0), '1' }, { (1, 0), '2' }, { (2, 0), '3' },
    { (0, 1), '4' }, { (1, 1), '5' }, { (2, 1), '6' },
    { (0, 2), '7' }, { (1, 2), '8' }, { (2, 2), '9' },
};

Dictionary<(int x, int y), char> part2KeyPad = new()
{
    { (2, 0), '1' },
    { (1, 1), '2' }, { (2, 1), '3' }, { (3, 1), '4' },
    { (0, 2), '5' }, { (1, 2), '6' }, { (2, 2), '7' }, { (3, 2), '8' }, { (4, 2), '9' },
    { (1, 3), 'A' }, { (2, 3), 'B' }, { (3, 3), 'C' },
    { (2, 4), 'D' }
};


Console.WriteLine($"Part 1: {FollowInstructions((1, 1), instructions, part1KeyPad)}");
Console.WriteLine($"Part 2: {FollowInstructions((0, 2), instructions, part2KeyPad)}");

static string FollowInstructions((int x, int y) pos, IEnumerable<Instruction> instructions, Dictionary<(int x, int y), char> keyPad)
{
    StringBuilder codeWIP = new();

    foreach (var instruction in instructions)
    {
        foreach (var step in instruction.Steps)
        {
            var newPos = step switch
            {
                Direction.Up => (pos.x, pos.y - 1),
                Direction.Down => (pos.x, pos.y + 1),
                Direction.Left => (pos.x - 1, pos.y),
                Direction.Right => (pos.x + 1, pos.y),
                _ => throw new Exception()
            };
            
            if (keyPad.ContainsKey(newPos))
            {
                pos = newPos;
            }
        }

        codeWIP.Append(keyPad[pos]);
    }

    return codeWIP.ToString();
}

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