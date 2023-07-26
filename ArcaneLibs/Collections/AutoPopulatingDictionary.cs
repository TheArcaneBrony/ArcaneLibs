namespace ArcaneLibs.Collections;

public class AutoPopulatingDictionary<T1, T2> : Dictionary<T1, T2> {
    private Dictionary<T1, T2> _backingStore = new();
    public bool Contains(KeyValuePair<T1, T2> item) => true;

    public bool ContainsKey(T1 key) => true;

    public T2 this[T1 key] {
        get {
            var a = typeof(T2).GetConstructors();
            if (!_backingStore.ContainsKey(key)) _backingStore.Add(key, GetNewInstance.Invoke());
            return _backingStore[key];
        }
        set {
            if (!_backingStore.ContainsKey(key)) _backingStore.Add(key, value);
            else _backingStore[key] = value;
        }
    }

    private Func<T2> GetNewInstance = () => {
        if (typeof(T2) == typeof(string)) return (T2)(object)"";
        return Activator.CreateInstance<T2>();
    };
}