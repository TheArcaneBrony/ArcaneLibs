namespace ArcaneLibs.Collections;

public class AutoPopulatingDictionary<T1, T2> : Dictionary<T1, T2> where T1 : notnull {
    private readonly Dictionary<T1, T2> _backingStore = new();
    public bool Contains(KeyValuePair<T1, T2> item) => true;

    public new bool ContainsKey(T1 key) => true;

    public new T2 this[T1 key] {
        get {
            if (!_backingStore.ContainsKey(key)) _backingStore.Add(key, GetNewInstance.Invoke());
            return _backingStore[key];
        }
        set => _backingStore[key] = value;
    }

    // ReSharper disable once MemberCanBePrivate.Global
    public Func<T2> GetNewInstance { get; set; } = () => {
        if (typeof(T2) == typeof(string)) return (T2)(object)"";
        return Activator.CreateInstance<T2>();
    };
}