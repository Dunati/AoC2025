
record struct V3i(int X, int Y, int Z)
{
    public V3i() : this(0, 0, 0) { }
    public V3i(int all) : this(all, all, all) { }
    public static implicit operator V3i((int x, int y, int z) a) => new V3i(a.x, a.y, a.z);

    public V3i Replace(V3i selector, V3i values)
    {
        return new V3i(selector.X == 0 ? values.X : X, selector.Y == 0 ? values.Y : Y, selector.Z == 0 ? values.Z : Z);
    }

    public V3i WithIndex(int index, int value)
    {
        return new V3i(index == 0 ? value : X, index == 1 ? value : Y, index == 2 ? value : Z);
    }


    public static V3i operator +(V3i a, V3i b)
    {
        return new V3i(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }
    public static V3i operator -(V3i a, V3i b)
    {
        return new V3i(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }

    public static V3i Min(V3i a, V3i b)
    {
        return (Math.Min(a.X, b.X), Math.Min(a.Y, b.Y), Math.Min(a.Z, b.Z));
    }

    public static V3i Max(V3i a, V3i b)
    {
        return (Math.Max(a.X, b.X), Math.Max(a.Y, b.Y), Math.Max(a.Z, b.Z));
    }

    public override string ToString()
    {
        return $"{X},{Y},{Z}";
    }

    public int this[int index]
    {
        get
        {
            return index switch
            {
                0 => X,
                1 => Y,
                2 => Z,
                _ => throw new IndexOutOfRangeException()
            };
        }
        set
        {
            switch (index)
            {
                case 0: X = value; break;
                case 1: Y = value; break;
                case 2: Z = value; break;
                default: throw new IndexOutOfRangeException();
            }
        }
    }

    public static V3i operator *(in V3i a, int v)
    {
        return new V3i(v * a.X, v * a.Y, v * a.Z);
    }

    public static readonly V3i Zero = new();
}