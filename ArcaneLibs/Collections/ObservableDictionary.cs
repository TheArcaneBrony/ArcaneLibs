using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;

namespace ArcaneLibs.Collections;

public class ObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, INotifyCollectionChanged, INotifyPropertyChanged where TKey : notnull {

    public ObservableDictionary() {
        Console.WriteLine("ObservableDictionary created");
    }
    private Dictionary<TKey, TValue> _dictionary = new();

    public ObservableDictionary(ObservableDictionary<TKey, TValue> value) {
        _dictionary = new Dictionary<TKey, TValue>(value);
    }

    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    public event PropertyChangedEventHandler PropertyChanged;

    public int Count => _dictionary.Count;
    public bool IsReadOnly => false;
    public ICollection<TKey> Keys => _dictionary.Keys;
    public ICollection<TValue> Values => _dictionary.Values;

    public void Add(KeyValuePair<TKey, TValue> item) => Add(item.Key, item.Value);
    public bool Contains(KeyValuePair<TKey, TValue> item) => _dictionary.Contains(item);
    public bool ContainsKey(TKey key) => _dictionary.ContainsKey(key);
    public bool TryGetValue(TKey key, out TValue value) => _dictionary.TryGetValue(key, out value);
    public bool Remove(KeyValuePair<TKey, TValue> item) => _dictionary.Remove(item.Key);

    void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int index) =>
        ((ICollection<KeyValuePair<TKey, TValue>>)_dictionary).CopyTo(array, index);

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => _dictionary.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _dictionary.GetEnumerator();

    public void Add(TKey key, TValue value) {
        _dictionary.Add(key, value);
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
    }

    public bool Remove(TKey key) {
        if (!_dictionary.ContainsKey(key)) return false;
        var value = _dictionary[key];
        var result = _dictionary.Remove(key);
        if (result) {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new KeyValuePair<TKey, TValue>(key, value)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        }

        return result;
    }

    public void Clear() {
        _dictionary.Clear();
        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
    }

    public TValue this[TKey key] {
        get {
            Console.WriteLine($"Getting value {key}");
            return _dictionary[key];
        }
        set {
            Console.WriteLine($"Setting value {key} -> {value} --- {ContainsKey(key)}");
            if (ContainsKey(key)) {
                var oldValue = _dictionary[key];
                _dictionary[key] = value;
                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new KeyValuePair<TKey, TValue>(key, value),
                        new KeyValuePair<TKey, TValue>(key, oldValue)));
            }
            else {
                _dictionary[key] = value;
                CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new KeyValuePair<TKey, TValue>(key, value)));
            }

            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
        }
    }

    public void AddRange(Dictionary<TKey, TValue> items) {
        foreach (var (key, value) in items) {
            Add(key, value);
        }

        CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, items));
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
    }
}