using System.Drawing;

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