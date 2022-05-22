using System.Drawing;
using System.Text.RegularExpressions;

Regex numRegex = new(@"\d+");
Regex hexColorRegex = new(@"#[0-9a-z][0-9a-z][0-9a-z][0-9a-z][0-9a-z][0-9a-z]");

var initialTilesParser = new SingleLineStringInputParser<ITile>(GetSetStateTileFromChar, str => str.ToCharArray().Select(w => w.ToString()).ToArray());
var initialTiles = new InputProvider<ITile>("Input.txt", initialTilesParser.GetValue).ToList();
var width = initialTiles.Count;


for (int x = 0; x < width; x++)
{
    var tile = (SetStateTile)initialTiles[x];
    tile.Position = new Point(x, 0);
}

List<ITile> lastTileRow = new List<ITile>(initialTiles);
int totalSafeCells = lastTileRow.Count(w => !w.IsTrap);

for (int row = 1; row < 400000; row++)
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

Console.WriteLine(totalSafeCells);

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

interface ITile : IWorldObject
{
    bool IsTrap { get; }
}

class SetStateTile : ITile
{
    public bool IsTrap { get; init; }

    public Point Position { get; set; }

    public char CharRepresentation => this.IsTrap ? '^' : '.';

    public int Z => 0;
}

class CalculatedStateTile : ITile
{
    private Cached<bool> cachedIsTrap;
    public bool IsTrap => this.cachedIsTrap.Value;

    public Point Position { get; init; }

    public char CharRepresentation => this.cachedIsTrap.Value ? '^' : '.';

    public int Z => 0;

    public ITile? Left { get; set; }
     
    public ITile? Center { get; set; }
     
    public ITile? Right { get; set; }

    public CalculatedStateTile()
    {
        cachedIsTrap = new Cached<bool>(GetIsTrapAndCleanUp);
    }

    private bool GetIsTrapAndCleanUp()
    {
        bool isTrap = GetIsTrap();

        this.Left = null;
        this.Center = null;
        this.Right = null;

        return isTrap;

        bool GetIsTrap()
        {
            if (this.cachedIsTrap.IsValueCreated)
                throw new Exception();

            //left and center but not right
            if ((this.Left?.IsTrap ?? false) && (this.Center?.IsTrap ?? false) && !(this.Right?.IsTrap ?? false))
                return true;

            //center and right but not left
            if (!(this.Left?.IsTrap ?? false) && (this.Center?.IsTrap ?? false) && (this.Right?.IsTrap ?? false))
                return true;

            // only left
            if ((this.Left?.IsTrap ?? false) && !(this.Center?.IsTrap ?? false) && !(this.Right?.IsTrap ?? false))
                return true;

            // only right
            if (!(this.Left?.IsTrap ?? false) && !(this.Center?.IsTrap ?? false) && (this.Right?.IsTrap ?? false))
                return true;

            return false;
        }
    }
}