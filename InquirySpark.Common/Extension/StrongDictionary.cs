using System.Runtime.Serialization;

namespace InquirySpark.Common.Extension;

/// <summary>
/// Special Dictionary for Use with Restful / AJAX Calls
/// </summary>
/// <typeparam name="TKey">The type of the t key.</typeparam>
/// <typeparam name="TValue">The type of the value.</typeparam>
public sealed class StrongDictionary<TKey, TValue>
{
    /// <summary>
    /// The dictionary
    /// </summary>
    private readonly Dictionary<TKey, TValue> _dictionary;

    /// <summary>
    /// Initializes a new instance of the <see cref="StrongDictionary{TKey, TValue}"/> class.
    /// </summary>
    public StrongDictionary() { _dictionary = []; }

    /// <summary>
    /// Initializes a new instance of the <see cref="StrongDictionary{TKey, TValue}"/> class.
    /// </summary>

    /// <summary>
    /// Gets or sets the <typeparamref name="TValue"/> with the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <returns>TValue.</returns>
    public TValue this[TKey key]
    {
        get
        {
            _dictionary.TryGetValue(key, out TValue vOut);
            return vOut;
        }
        set
        {
            _dictionary.TryGetValue(key, out TValue vOut);
            if (vOut == null)
            {
                _dictionary.Add(key, value);
            }
            else
            {
                _dictionary[key] = value;
            }
        }
    }

    /// <summary>
    /// Adds the specified dictionary into the current dictionary
    /// </summary>
    /// <param name="value">The value.</param>
    public void Add(Dictionary<TKey, TValue> value)
    {
        foreach (var item in value.Keys)
        {
            Add(item, value[item]);
        }
    }

    /// <summary>
    /// Adds the specified key.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <param name="value">The value.</param>
    public void Add(TKey key, TValue value)
    {
        if (_dictionary.ContainsKey(key))
            _dictionary[key] = value;
        else
            _dictionary.Add(key, value);
    }

    /// <summary>
    /// Gets the list.
    /// </summary>
    /// <returns>List&lt;System.String&gt;.</returns>
    public List<string> GetList()
    {
        List<string> list = [];
        foreach (var item in _dictionary)
        {
            list.Add($"{item.Key} - {item.Value}");
        }
        return list;
    }

    /// <summary>
    /// Gets the object data.
    /// </summary>
    /// <param name="info">The information.</param>
    public void GetObjectData(SerializationInfo info)
    {
        foreach (TKey key in _dictionary.Keys)
        {
            if (key != null)
            {
                info.AddValue(key.ToString(), _dictionary[key]);
            }
        }
    }
}
