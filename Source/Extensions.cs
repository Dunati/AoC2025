public static class Extensions
{
    public static bool IsPositivePowerOfTwo(this int value)
    {
        return value > 0 && (value & (value - 1)) == 0;
    }
    public static int NextPowerOfTwo(this int value)
    {
        return 1 << ((int)Log2(value) + 1);
    }
    public static int[][] ToJaggedDigitArray(this string str)
    {
        return str.Lines().Select(x => x.Select(x => x - '0').ToArray()).ToArray();
    }

    public static T[,] To2DArray<T>(this T[][] src)
    {
        int w = src[0].Length;
        int h = src.Length;

        T[,] result = new T[w, h];

        for (int y = 0; y < h; y++)
        {
            for (int x = 0; x < w; x++)
            {
                result[x, y] = src[y][x];
            }
        }
        return result;
    }

    public static unsafe void Fill(this int[,] array, int value)
    {
        fixed (int* ptr = &array[0, 0])
        {
            int* curr = ptr;
            int* last = ptr + array.GetLength(0) * array.GetLength(1);
            while (curr < last)
            {
                *curr++ = value;
            }
        }
    }

    public static int[,] ToDigitArray(this string str)
    {
        return str.Lines().Select(x => x.Select(x => x - '0').ToArray()).ToArray().To2DArray();
    }


    public static IEnumerable<double> ToDoubles(this string str, string separator = "\r\n")
    {
        return str.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToDoubles(separator);
    }
    public static IEnumerable<double> ToDoubles(this IEnumerable<string> str, string separator = "\r\n")
    {
        return str.Select(x => Convert.ToDouble(x));
    }

    public static IEnumerable<Int16> ToShorts(this string str, int @base = 10, string separator = "\r\n")
    {
        return str.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToShorts(@base, separator);
    }
    public static IEnumerable<Int16> ToShorts(this IEnumerable<string> str, int @base = 10, string separator = "\r\n")
    {
        return str.Select(x => Convert.ToInt16(x, @base));
    }

    public static IEnumerable<int> ToInts(this string str, int @base = 10, string separator = "\r\n")
    {
        return str.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToInts(@base, separator);
    }
    public static IEnumerable<int> ToInts(this IEnumerable<string> str, int @base = 10, string separator = "\r\n")
    {
        return str.Select(x => Convert.ToInt32(x, @base));
    }

    public static IEnumerable<Int64> ToInt64s(this string str, int @base = 10, string separator = "\r\n")
    {
        return str.Split(separator.ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToInt64s(@base, separator);
    }
    public static IEnumerable<Int64> ToInt64s(this IEnumerable<string> str, int @base = 10, string separator = "\r\n")
    {
        return str.Select(x => Convert.ToInt64(x, @base));
    }
    public static IEnumerable<int> ToSortedInt(this string str, int @base = 10, string separator = "\r\n")
    {
        return str.ToInts(@base, separator).OrderBy(x => x).ToArray();
    }
    public static IEnumerable<int> ToSortedInt(this IEnumerable<string> str, int @base = 10, string separator = "\r\n")
    {
        return str.ToInts(@base, separator).OrderBy(x => x).ToArray();
    }
    public static IEnumerable<String> Lines(this string str)
    {
        return str.Split("\r\n".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
    }

    // https://stackoverflow.com/questions/47815660/does-c-sharp-7-have-array-enumerable-destructuring
    public static void Deconstruct<T>(this IEnumerable<T> list, out T first, out IEnumerable<T> rest)
    {
        first = list.First();
        rest = list.Skip(1);
    }

    public static void Deconstruct<T>(this IEnumerable<T> list, out T first, out T second, out IEnumerable<T> rest)
    {
        first = list.First();
        second = list.Skip(1).First();
        rest = list.Skip(2);
    }
    public static void Deconstruct<T>(this IEnumerable<T> list, out T first, out T second, out T third, out IEnumerable<T> rest)
    {
        first = list.First();
        second = list.Skip(1).First();
        third = list.Skip(2).First();
        rest = list.Skip(3);
    }
    public static void Deconstruct<T>(this IEnumerable<T> list, out T first, out T second, out T third, out T fourth, out IEnumerable<T> rest)
    {
        first = list.First();
        second = list.Skip(1).First();
        third = list.Skip(2).First();
        fourth = list.Skip(3).First();
        rest = list.Skip(4);
    }

    public static T Min<T>(this Span<T> value) where T : IComparable<T>
    {
        T min = value[0];
        for (int i = 1; i < value.Length; i++)
        {
            if (value[i].CompareTo(min) < 0)
            {
                min = value[i];
            }
        }
        return min;
    }
    public static T Max<T>(this Span<T> value) where T : IComparable<T>
    {
        T max = value[0];
        for (int i = 1; i < value.Length; i++)
        {
            if (value[i].CompareTo(max) > 0)
            {
                max = value[i];
            }
        }
        return max;
    }

    public static T Pop<T>(this List<T> list)
    {
        int last = list.Count - 1;
        T value = list[last];
        list.RemoveAt(last);
        return value;
    }

    public static void Push<T>(this List<T> list, T value)
    {
        list.Add(value);
    }

    public static IEnumerable<T[]> Permute<T>(this IEnumerable<T> source)
    {
        return permutate(source, Enumerable.Empty<T>());

        IEnumerable<T[]> permutate(IEnumerable<T> reminder, IEnumerable<T> prefix)
        {
            if (reminder.Any())
            {
                return
                    from t in reminder.Select((r, i) => (r, i))
                    let nextReminder = reminder.Take(t.i).Concat(reminder.Skip(t.i + 1)).ToArray()
                    let nextPrefix = prefix.Append(t.r)
                    from permutation in permutate(nextReminder, nextPrefix)
                    select permutation;

            }
            else
            {
                return new[] { prefix.ToArray() };
            }
        }
    }


    public static Point Add(this in Point p, in Point o)
    {
        return new System.Drawing.Point(p.X + o.X, p.Y + o.Y);
    }


    public static BitArray ZeroExtend(this BitArray b, int bits)
    {
        BitArray n = new BitArray(bits);
        {
            for (int i = 0; i < Math.Min(b.Length, bits); i++)
            {
                n[i] = b[i];
            }
        }
        return n;
    }

    public static uint Extract(this BitArray b, int first, int count)
    {
        uint result = 0;
        first = b.Length - first - 1;
        for (int i = 0; i < count; i++)
        {
            result <<= 1;
            result |= b[first - i] ? 1u : 0u;
        }
        return result;
    }

    public static string ToBinaryString(this BitArray b)
    {
        StringBuilder sb = new StringBuilder(b.Length);

        for (int i = b.Length - 1; i >= 0; i--)
        {
            sb.Append(b[i] ? '1' : '0');
        }
        return sb.ToString();
    }
}