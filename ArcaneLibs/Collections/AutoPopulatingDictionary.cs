namespace ArcaneLibs.Collections;

public class AutoPopulatingDictionary<T1, T2> : Dictionary<T1, T2> {
    private Dictionary<T1, T2> _backingStore = new();
    public bool Contains(KeyValuePair<T1, T2> item) => true;

    public bool ContainsKey(T1 key) => true;

    public T2 this[T1 key] {
        get {
            var a = typeof(T2).GetConstructors();
            return _backingStore[key];
        }
        set => _backingStore[key] = value;
    }
    
    public Func<T1, T2> GetNewInstance = (T1 key) => {
        if (typeof(T2) == typeof(string)) return (T2)(object)"";
        return Activator.CreateInstance<T2>();
    };
}
