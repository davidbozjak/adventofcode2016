using System.Text.RegularExpressions;

var instructions = new InputProvider<TriangleDescription?>("Input.txt", GetTriangleDescriptions)
    .Where(w => w != null).Cast<TriangleDescription>()
    .ToList();

Console.WriteLine($"Part 1: {instructions.Where(IsValidTriangle).Count()}");

List<TriangleDescription> verticalTriangles = new();

for(int i = 0; i < instructions.Count; i += 3)
{
    verticalTriangles.Add(new TriangleDescription(instructions[i].SideA, instructions[i + 1].SideA, instructions[i + 2].SideA));
    verticalTriangles.Add(new TriangleDescription(instructions[i].SideB, instructions[i + 1].SideB, instructions[i + 2].SideB));
    verticalTriangles.Add(new TriangleDescription(instructions[i].SideC, instructions[i + 1].SideC, instructions[i + 2].SideC));
}

Console.WriteLine($"Part 2: {verticalTriangles.Where(IsValidTriangle).Count()}");

static bool IsValidTriangle(TriangleDescription triangle)
{
    if ((triangle.SideA < (triangle.SideB + triangle.SideC)) &&
        (triangle.SideB < (triangle.SideA + triangle.SideC)) &&
        (triangle.SideC < (triangle.SideA + triangle.SideB)))
            return true;

    return false;
}

static bool GetTriangleDescriptions(string? input, out TriangleDescription? value)
{
    value = null;

    if (input == null) return false;

    Regex numRegex = new(@"\d+");
    var matches = numRegex.Matches(input)
        .Select(w => int.Parse(w.Value))
        .ToArray();

    if (matches.Length != 3) throw new Exception();

    value = new TriangleDescription(matches[0], matches[1], matches[2]);

    return true;
}

record TriangleDescription (int SideA, int SideB, int SideC);