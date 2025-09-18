namespace ArcaneLibs;

public class ObservableValue<T>(T value = default!) {
    public static implicit operator T(ObservableValue<T> ov) => ov.Value;
    public static implicit operator ObservableValue<T>(T v) => new(v);

    private T _value = value;

    public T Value {
        get => _value;
        set {
            if (EqualityComparer<T>.Default.Equals(_value, value)) return;
            // var old = _value;
            _value = value;
            ValueChanged?.Invoke(this, _value);
        }
    }

    public T SetValue(T value, bool inhibitEvent = false) {
        if (EqualityComparer<T>.Default.Equals(_value, value)) return _value;
        // var old = field;
        _value = value;
        if (!inhibitEvent)
            ValueChanged?.Invoke(this, _value);
        return _value;
    }

    public event EventHandler<T>? ValueChanged;

    // public class ValueChangeEventArgs<T>(T oldValue, T newValue) : EventArgs {
    //     public T OldValue { get; } = oldValue;
    //     public T NewValue { get; } = newValue;
    // }
}