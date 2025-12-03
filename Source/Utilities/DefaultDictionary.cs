

public class DefaultDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
{

    public bool ContainsKey(TKey index)
    {
        return entries.ContainsKey(index);
    }

    public Dictionary<TKey, TValue>.ValueCollection Values => entries.Values;

    public Dictionary<TKey, TValue>.KeyCollection Keys => entries.Keys;

    public TValue Get(TKey index, Func<TValue> @default)
    {
        TValue value = default(TValue);
        if (!entries.TryGetValue(index, out value))
        {
            value = @default();
            entries[index] = value;
        }

        return value;
    }

    public Func<TValue> Default { get; set; } = () => { return default(TValue); };
    public bool InsertOnDefaultReference { get; set; } = false;

    public TValue this[TKey index]
    {
        get
        {
            if (entries.TryGetValue(index, out var value))
            {
                return value;
            }
            var new_value = Default();
            if (InsertOnDefaultReference)
            {
                entries[index] = new_value;
            }
            return new_value;
        }
        set
        {
            if (value.Equals(Default()))
            {
                entries.Remove(index);
            }
            else
            {
                entries[index] = value;
            }
        }
    }

    public int Count => entries.Count;

    public DefaultDictionary<TKey, TValue> Clone()
    {
        var dict = new DefaultDictionary<TKey, TValue>();

        dict.entries = entries.ToDictionary(x => x.Key, x => x.Value);
        return dict;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return ((IEnumerable<KeyValuePair<TKey, TValue>>)entries).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)entries).GetEnumerator();
    }

    private Dictionary<TKey, TValue> entries = new Dictionary<TKey, TValue>();
}