using System.Drawing;

var initialTilesParser = new SingleLineStringInputParser<ITile>(GetSetStateTileFromChar, str => str.ToCharArray().Select(w => w.ToString()).ToArray());
var initialTiles = new InputProvider<ITile>("Input.txt", initialTilesParser.GetValue).ToList();
var width = initialTiles.Count;

for (int x = 0; x < width; x++)
{
    var tile = (SetStateTile)initialTiles[x];
    tile.Position = new Point(x, 0);
}

Console.WriteLine($"Part 1: {GenerateTileGridFromInitialTiles(initialTiles, 40)}");
Console.WriteLine($"Part 2: {GenerateTileGridFromInitialTiles(initialTiles, 400000)}");

static bool GetSetStateTileFromChar(string? input, out ITile? value)
{
    value = null;

    if (string.IsNullOrWhiteSpace(input))
        return false;

    value = new SetStateTile()
    {
        IsTrap = input[0] == '^'
    };

    return true;
}

static int GenerateTileGridFromInitialTiles(List<ITile> initialTiles, int rowsToGenerate)
{
    int width = initialTiles.Count;
    List<ITile> lastTileRow = new List<ITile>(initialTiles);
    int totalSafeCells = lastTileRow.Count(w => !w.IsTrap);

    for (int row = 1; row < rowsToGenerate; row++)
    {
        var newRow = new List<ITile>(lastTileRow.Count);

        for (int x = 0; x < width; x++)
        {
            var tile = new CalculatedStateTile()
            {
                Position = new Point(x, row),
                Left = x > 0 ? lastTileRow[x - 1] : null,
                Center = lastTileRow[x],
                Right = x < width - 1 ? lastTileRow[x + 1] : null,

            };

            newRow.Add(tile);
        }

        lastTileRow = newRow;
        totalSafeCells += lastTileRow.Count(w => !w.IsTrap);
    }

    return totalSafeCells;
}

