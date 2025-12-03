
record struct Bounds3i(V3i min, V3i max)
{
    public decimal Volume => ((decimal)max.X - min.X) * (max.Y - min.Y) * (max.Z - min.Z);
    public Bounds3i Encapsulate(V3i p)
    {
        return new Bounds3i()
        {
            min = new V3i(Min(min.X, p.X), Min(min.Y, p.Y), Min(min.Z, p.Z)),
            max = new V3i(Max(max.X, p.X), Max(max.Y, p.Y), Max(max.Z, p.Z)),
        };
    }


    public IEnumerable<V3i> Corners()
    {
        yield return (min.X, min.Y, min.Z);

        yield return (min.X, min.Y, max.Z);
        yield return (min.X, max.Y, min.Z);
        yield return (max.X, min.Y, min.Z);


        yield return (min.X, max.Y, max.Z);
        yield return (max.X, min.Y, max.Z);
        yield return (max.X, max.Y, min.Z);

        yield return (max.X, max.Y, max.Z);
    }


    public Bounds3i(V3i center, int halfSize) : this(center - new V3i(halfSize), center + new V3i(halfSize)) { }

    public bool IsCube => max.X - min.X == max.Y - min.Y && max.X - min.X == max.Z - min.Z;

    public V3i Center => ((max.X - min.X) / 2 + min.X, (max.Y - min.Y) / 2 + min.Y, (max.Z - min.Z) / 2 + min.Z);

    public bool Contains(V3i p)
    {
        return
            p.X >= min.X && p.X <= max.X &&
            p.Y >= min.Y && p.Y <= max.Y &&
            p.Z >= min.Z && p.Z <= max.Z;
    }

    public bool Contains(Bounds3i p)
    {
        return Contains(p.min) && Contains(p.max);
    }

    public bool Overlaps(Bounds3i b)
    {
        return Intersect(b).VolumePositive;
    }

    public Bounds3i Intersect(Bounds3i b)
    {
        Bounds3i n = new Bounds3i(
            V3i.Max(min, b.min),
            V3i.Min(max, b.max));

        return n;
    }

    public IEnumerable<Bounds3i> Split(V3i values, int index)
    {
        int value = values[index];

        if(value > max[index] || value < min[index])
        {
            yield return this;
        }
        else
        {

            if (value > min[index])
            {
                yield return new Bounds3i(min, max.WithIndex(index,value));
            }
            if(value < max[index])
            {
                yield return new Bounds3i(min.WithIndex(index, value), max);
            }
        }
    }


    public Bounds3i[] SplitX(int value)
    {
        if (value > max.X)
        {
            return new Bounds3i[] { this, default };
        }
        if (value < min.X)
        {
            return new Bounds3i[] { default, this };
        }
        return new Bounds3i[]
        {
            value > min.X?LowX(value):default,
            value < max.X?HighX(value):default,
        };
    }


    public Bounds3i LowX(int value)
    {
        return new Bounds3i((min.X, min.Y, min.Z), (value, max.Y, max.Z));
    }
    public Bounds3i HighX(int value)
    {
        return new Bounds3i((value, min.Y, min.Z), (max.X, max.Y, max.Z));
    }

    public Bounds3i[] SplitY(int value)
    {
        if (value > max.Y)
        {
            return new Bounds3i[] { this, default };
        }
        if (value < min.Y)
        {
            return new Bounds3i[] { default, this };
        }
        return new Bounds3i[]
        {
            value > min.Y?LowY(value):default,
            value < max.Y?HighY(value):default,
        };
    }

    public readonly Bounds3i HighY(int value)
    {
        return new Bounds3i((min.X, value, min.Z), (max.X, max.Y, max.Z));
    }

    public readonly Bounds3i LowY(int value)
    {
        return new Bounds3i((min.X, min.Y, min.Z), (max.X, value, max.Z));
    }

    public Bounds3i[] SplitZ(int value)
    {
        if (value > max.Z)
        {
            return new Bounds3i[] { this, default };
        }
        if (value < min.Z)
        {
            return new Bounds3i[] { default, this };
        }
        return new Bounds3i[]
        {
            value > min.Z?LowZ(value):default,
            value < max.Z?HighZ(value):default,
        };
    }

    public readonly Bounds3i HighZ(int value)
    {
        return new Bounds3i((min.X, min.Y, value), (max.X, max.Y, max.Z));
    }

    public readonly Bounds3i LowZ(int value)
    {
        return new Bounds3i((min.X, min.Y, min.Z), (max.X, max.Y, value));
    }

    public Bounds3i[] Split(V3i p)
    {
        var b = new Bounds3i[8];

        int index = 0;

        foreach (var x in SplitX(p.X))
        {
            foreach (var y in x.SplitY(p.Y))
            {
                foreach (var z in y.SplitZ(p.Z))
                {
                    b[index++] = z;
                }
            }
        }
        return b;

    }

    public V3i Size()
    {
        return new V3i(max.X - min.X, max.Y - min.Y, max.Z - min.Z);
    }

    public bool VolumePositive => (max.X > min.X) && (max.Y > min.Y) && (max.Z > min.Z);
}
