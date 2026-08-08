using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;

namespace Cubusky.Stats.Core;

public partial interface IStat<TValue>
    : IAutoObject<Stat<TValue>.Binding>,
    IDisposable
{
    TValue Value { get; }
    TValue Min { get; }
    TValue Max { get; }
    IComparer<TValue> Comparer { get; }
}

public partial class Stat<TValue> : IStat<TValue>,
    IPerform<Stat<TValue>.SetValueOp>,
    IPerform<Stat<TValue>.SetMinOp>,
    IPerform<Stat<TValue>.SetMaxOp>,
    IPerform<Stat<TValue>.SyncValueOp>,
    IPerform<Stat<TValue>.SyncMinOp>,
    IPerform<Stat<TValue>.SyncMaxOp>,
    IPerform<Stat<TValue>.SyncIsMinOp>,
    IPerform<Stat<TValue>.SyncIsMaxOp>,
    IAutoObject<Stat<TValue>.Binding>,
    IDisposable
{
    public Stat(TValue value, TValue min, TValue max, IComparer<TValue>? comparer = null)
    {
        Comparer = comparer ?? Comparer<TValue>.Default;

        if (Comparer.Compare(value, min) < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Value {value} cannot be less than min value {min} on {GetType()} initialization.");
        }

        if (Comparer.Compare(value, max) > 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Value {value} cannot be greater than max value {max} on {GetType()} initialization.");
        }

        if (Comparer.Compare(min, max) > 0)
        {
            throw new ArgumentOutOfRangeException(nameof(min), min, $"Min value {min} cannot be greater than max value {max} on {GetType()} initialization.");
        }

        _subject = new(this);
        _value = value;
        _min = min;
        _max = max;
    }

    #region Values
    protected readonly SyncSubject _subject;

    private TValue _value;
    public TValue Value
    {
        get => _value;
        set => _subject.Perform(new SetValueOp(value));
    }

    private TValue _min;
    public TValue Min
    {
        get => _min;
        set => _subject.Perform(new SetMinOp(value));
    }

    private TValue _max;
    public TValue Max
    {
        get => _max;
        set => _subject.Perform(new SetMaxOp(value));
    }

    public IComparer<TValue> Comparer { get; }
    #endregion

    #region Operations
    private readonly record struct SetValueOp(TValue Value);
    private readonly record struct SetMinOp(TValue Value);
    private readonly record struct SetMaxOp(TValue Value);

    private readonly record struct SyncValueOp(Action<TValue> Callback, Func<TValue, bool> Condition);
    private readonly record struct SyncMinOp(Action<TValue> Callback, Func<TValue, bool> Condition);
    private readonly record struct SyncMaxOp(Action<TValue> Callback, Func<TValue, bool> Condition);
    private readonly record struct SyncIsMinOp(Action<TValue> Callback, Func<TValue, bool> Condition);
    private readonly record struct SyncIsMaxOp(Action<TValue> Callback, Func<TValue, bool> Condition);
    #endregion

    #region Broadcast
    private readonly record struct ValueBroadcast(TValue Value);
    private readonly record struct MinBroadcast(TValue Value);
    private readonly record struct MaxBroadcast(TValue Value);
    private readonly record struct IsMinBroadcast(TValue Value);
    private readonly record struct IsMaxBroadcast(TValue Value);
    #endregion

    #region Perform
    void IPerform<SetValueOp>.Perform(in SetValueOp op)
    {
        var clamped = Comparer.Clamp(op.Value, Min, Max);
        if (Comparer.Equals(_value, clamped))
        {
            return;
        }

        _subject.Broadcast(new ValueBroadcast(_value = clamped));

        if (Comparer.Equals(_value, Min))
        {
            _subject.Broadcast(new IsMinBroadcast(_value));
        }

        if (Comparer.Equals(_value, Max))
        {
            _subject.Broadcast(new IsMaxBroadcast(_value));
        }
    }

    void IPerform<SetMinOp>.Perform(in SetMinOp op)
    {
        var min = Comparer.Min(op.Value, Max);
        if (Comparer.Equals(_min, min))
        {
            return;
        }

        _subject.Broadcast(new MinBroadcast(_min = min));
        _subject.Perform(new SetValueOp(_value));
    }

    void IPerform<SetMaxOp>.Perform(in SetMaxOp op)
    {
        var max = Comparer.Max(op.Value, Min);
        if (Comparer.Equals(_max, max))
        {
            return;
        }

        _subject.Broadcast(new MaxBroadcast(_max = max));
        _subject.Perform(new SetValueOp(_value));
    }

    void IPerform<SyncValueOp>.Perform(in SyncValueOp op)
    {
        if (op.Condition(_value))
        {
            op.Callback(_value);
        }
    }

    void IPerform<SyncMinOp>.Perform(in SyncMinOp op)
    {
        if (op.Condition(_min))
        {
            op.Callback(_min);
        }
    }

    void IPerform<SyncMaxOp>.Perform(in SyncMaxOp op)
    {
        if (op.Condition(_max))
        {
            op.Callback(_max);
        }
    }

    void IPerform<SyncIsMinOp>.Perform(in SyncIsMinOp op)
    {
        if (op.Condition(_min))
        {
            op.Callback(_min);
        }
    }

    void IPerform<SyncIsMaxOp>.Perform(in SyncIsMaxOp op)
    {
        if (op.Condition(_max))
        {
            op.Callback(_max);
        }
    }
    #endregion

    #region Binding
    public Binding Bind() => new(_subject);
    public void ClearBindings() => _subject.ClearBindings();
    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _subject.Dispose();
    }

    public partial class Binding : SyncBinding
    {
        internal Binding(ISyncSubject subject) : base(subject) { }

        public Binding OnValue(Action<TValue> callback, Func<TValue, bool>? condition = null)
        {
            AddCallback
            (
                delegate (in ValueBroadcast broadcast) { callback(broadcast.Value); },
                delegate (in ValueBroadcast broadcast) { return predicate(broadcast.Value); }
            );

            _subject!.Perform(new SyncValueOp(callback, predicate));
            return this;

            bool predicate(TValue value) => condition?.Invoke(value) ?? true;
        }

        public Binding OnMin(Action<TValue> callback, Func<TValue, bool>? condition = null)
        {
            AddCallback
            (
                delegate (in MinBroadcast broadcast) { callback(broadcast.Value); },
                delegate (in MinBroadcast broadcast) { return predicate(broadcast.Value); }
            );

            _subject!.Perform(new SyncMinOp(callback, predicate));
            return this;

            bool predicate(TValue value) => condition?.Invoke(value) ?? true;
        }

        public Binding OnMax(Action<TValue> callback, Func<TValue, bool>? condition = null)
        {
            AddCallback
            (
                delegate (in MaxBroadcast broadcast) { callback(broadcast.Value); },
                delegate (in MaxBroadcast broadcast) { return predicate(broadcast.Value); }
            );

            _subject!.Perform(new SyncMaxOp(callback, predicate));
            return this;

            bool predicate(TValue value) => condition?.Invoke(value) ?? true;
        }

        public Binding OnIsMin(Action<TValue> callback, Func<TValue, bool>? condition = null)
        {
            AddCallback
            (
                delegate (in IsMinBroadcast broadcast) { callback(broadcast.Value); },
                delegate (in IsMinBroadcast broadcast) { return predicate(broadcast.Value); }
            );

            _subject!.Perform(new SyncIsMinOp(callback, predicate));
            return this;

            bool predicate(TValue value) => condition?.Invoke(value) ?? true;
        }

        public Binding OnIsMax(Action<TValue> callback, Func<TValue, bool>? condition = null)
        {
            AddCallback
            (
                delegate (in IsMaxBroadcast broadcast) { callback(broadcast.Value); },
                delegate (in IsMaxBroadcast broadcast) { return predicate(broadcast.Value); }
            );

            _subject!.Perform(new SyncIsMaxOp(callback, predicate));
            return this;

            bool predicate(TValue value) => condition?.Invoke(value) ?? true;
        }
    }
    #endregion
}

internal static class ComparerExtensions
{
    public static bool Equals<TValue>(this IComparer<TValue> comparer, TValue a, TValue b) => comparer.Compare(a, b) == 0;
    public static TValue Min<TValue>(this IComparer<TValue> comparer, TValue a, TValue b) => comparer.Compare(a, b) < 0 ? a : b;
    public static TValue Max<TValue>(this IComparer<TValue> comparer, TValue a, TValue b) => comparer.Compare(a, b) > 0 ? a : b;
    public static TValue Clamp<TValue>(this IComparer<TValue> comparer, TValue value, TValue min, TValue max) => comparer.Min(comparer.Max(value, min), max);
}

public static class StatExtensions
{
    public static bool TrySet<TValue>(this Stat<TValue> stat, TValue value, out TValue oldValue)
    {
        oldValue = stat.Value;
        stat.Value = value;
        return !stat.Comparer.Equals(oldValue, value);
    }

    public static bool TrySetMin<TValue>(this Stat<TValue> stat, TValue min, out TValue oldMin)
    {
        oldMin = stat.Min;
        stat.Min = min;
        return !stat.Comparer.Equals(oldMin, min);
    }

    public static bool TrySetMax<TValue>(this Stat<TValue> stat, TValue max, out TValue oldMax)
    {
        oldMax = stat.Max;
        stat.Max = max;
        return !stat.Comparer.Equals(oldMax, max);
    }
}