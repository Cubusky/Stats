using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats;

public interface IStat<TValue> : IOperator<Stat<TValue>>, IAutoObject<Stat<TValue>.Binding>, IDisposable
{
    TValue? Value { get; }
    TValue? Min { get; }
    TValue? Max { get; }
    IComparer<TValue> Comparer { get; }
    OperationMap OperationMap { get; }
}

public class Stat<TValue> : IStat<TValue>, IPerform<Stat<TValue>.SetValueOp>, IPerform<Stat<TValue>.SetMinOp>, IPerform<Stat<TValue>.SetMaxOp>, IPerform<Stat<TValue>.SyncValueOp>, IPerform<Stat<TValue>.SyncMinOp>, IPerform<Stat<TValue>.SyncMaxOp>, IPerform<Stat<TValue>.SyncIsMinOp>, IPerform<Stat<TValue>.SyncIsMaxOp>, IAutoObject<Stat<TValue>.Binding>, IDisposable
{
    public static implicit operator Stat<TValue>(TValue value) => new(value);

    public Stat(TValue? value, IComparer<TValue>? comparer = null)
        : this(value, value, comparer) { }

    public Stat(TValue? value, TValue? max, IComparer<TValue>? comparer = null)
        : this(value, default, max, comparer) { }

    public Stat(TValue? value, TValue? min, TValue? max, IComparer<TValue>? comparer = null)
    {
        Comparer = comparer ?? Comparer<TValue>.Default;

        if (Comparer.Compare(value!, min!) < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Value {value} cannot be less than min value {min} on {GetType()} initialization.");
        }

        if (Comparer.Compare(value!, max!) > 0)
        {
            throw new ArgumentOutOfRangeException(nameof(value), value, $"Value {value} cannot be greater than max value {max} on {GetType()} initialization.");
        }

        if (Comparer.Compare(min!, max!) > 0)
        {
            throw new ArgumentOutOfRangeException(nameof(min), min, $"Min value {min} cannot be greater than max value {max} on {GetType()} initialization.");
        }

        Subject = new(this);
        _value = value;
        _min = min;
        _max = max;
    }

    #region Values
    private bool _isDisposed;
    protected internal SyncSubject Subject { get; }

    protected TValue? _value;
    public TValue? Value
    {
        get => _value;
        set => Subject.Perform(new SetValueOp(value));
    }

    protected TValue? _min;
    public TValue? Min
    {
        get => _min;
        set => Subject.Perform(new SetMinOp(value));
    }

    protected TValue? _max;
    public TValue? Max
    {
        get => _max;
        set => Subject.Perform(new SetMaxOp(value));
    }

    public IComparer<TValue> Comparer { get; }
    public OperationMap OperationMap { get; } = new();
    #endregion

    #region Operations
    protected readonly record struct SetValueOp(TValue? Value);

    protected readonly record struct SetMinOp(TValue? Value);

    protected readonly record struct SetMaxOp(TValue? Value);

    private readonly record struct SyncValueOp(Action<TValue?> Callback, Func<TValue?, bool> Condition);

    private readonly record struct SyncMinOp(Action<TValue?> Callback, Func<TValue?, bool> Condition);

    private readonly record struct SyncMaxOp(Action<TValue?> Callback, Func<TValue?, bool> Condition);

    private readonly record struct SyncIsMinOp(Action<TValue?> Callback, Func<TValue?, bool> Condition);

    private readonly record struct SyncIsMaxOp(Action<TValue?> Callback, Func<TValue?, bool> Condition);
    #endregion

    #region Broadcast
    private readonly record struct ValueBroadcast(TValue? Value);

    private readonly record struct MinBroadcast(TValue? Value);

    private readonly record struct MaxBroadcast(TValue? Value);

    private readonly record struct IsMinBroadcast(TValue? Value);

    private readonly record struct IsMaxBroadcast(TValue? Value);
    #endregion

    #region Perform
    internal protected void Perform<TConformance, TOperation, TBroadcaster>(in TConformance owner, in TOperation operation, in TBroadcaster broadcaster)
        where TConformance : class
        where TOperation : struct, IOperation<TConformance>
        where TBroadcaster : Broadcaster
    {
        Initializer<TOperation>.Ensure();
        var operationCallback = OperationMap.GetValueOrDefault<TConformance, TOperation, TBroadcaster>() ?? OperationMap.Shared.Get<TConformance, TOperation, TBroadcaster>();
        operationCallback(owner, operation, broadcaster);
    }

    void IOperator<Stat<TValue>>.Perform<TOperation>(in TOperation operation) => Perform(this, operation, new Broadcaster(Subject));

    void IPerform<SetValueOp>.Perform(in SetValueOp op) => Perform(op);

    protected virtual void Perform(in SetValueOp op)
    {
        var clamped = Comparer.Clamp(op.Value, Min, Max);
        if (Comparer.Equals(_value, clamped))
        {
            return;
        }

        Subject.Broadcast(new ValueBroadcast(_value = clamped));

        if (Comparer.Equals(_value, Min))
        {
            Subject.Broadcast(new IsMinBroadcast(_value));
        }

        if (Comparer.Equals(_value, Max))
        {
            Subject.Broadcast(new IsMaxBroadcast(_value));
        }
    }

    void IPerform<SetMinOp>.Perform(in SetMinOp op) => Perform(op);

    protected virtual void Perform(in SetMinOp op)
    {
        var min = Comparer.Min(op.Value, Max);
        if (Comparer.Equals(_min, min))
        {
            return;
        }

        Subject.Broadcast(new MinBroadcast(_min = min));

        if (Comparer.Compare(_value, _min) < 0)
        {
            Subject.Perform(new SetValueOp(_min));
        }
    }

    void IPerform<SetMaxOp>.Perform(in SetMaxOp op) => Perform(op);

    protected virtual void Perform(in SetMaxOp op)
    {
        var max = Comparer.Max(op.Value, Min);
        if (Comparer.Equals(_max, max))
        {
            return;
        }

        Subject.Broadcast(new MaxBroadcast(_max = max));

        if (Comparer.Compare(_value, _max) > 0)
        {
            Subject.Perform(new SetValueOp(_max));
        }
    }

    // ZA0504 false positive: the Sync*Op types are readonly record structs, so no defensive copy occurs.
    // The analyzer flags the delegate Invoke call on the 'in' parameter because Invoke is never marked readonly.
#pragma warning disable ZA0504
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
#pragma warning restore ZA0504

    #endregion

    #region Broadcaster
    public class Broadcaster
    {
        protected internal SyncSubject Subject { get; }

        protected internal Broadcaster(SyncSubject subject) => Subject = subject;

        public void Broadcast<TOperation>(in TOperation operation)
            where TOperation : struct, IOperation<Stat<TValue>> => Subject.Broadcast(in operation);
    }
    #endregion

    #region Binding
    public Binding Bind() => new(Subject);

    public void ClearBindings() => Subject.ClearBindings();

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_isDisposed)
        {
            _isDisposed = true;

            if (disposing)
            {
                Subject.Dispose();
            }
        }
    }

    public class Binding : SyncBinding
    {
        protected internal Binding(ISyncSubject subject)
            : base(subject) { }

        public Binding OnValue(Action<TValue?> callback, Func<TValue?, bool>? condition = null)
        {
            AddCallback(
                delegate(in ValueBroadcast broadcast)
                {
                    callback(broadcast.Value);
                },
                delegate(in ValueBroadcast broadcast)
                {
                    return predicate(broadcast.Value);
                }
            );

            _subject!.Perform(new SyncValueOp(callback, predicate));
            return this;

            bool predicate(TValue? value) => condition?.Invoke(value) ?? true;
        }

        public Binding OnMin(Action<TValue?> callback, Func<TValue?, bool>? condition = null)
        {
            AddCallback(
                delegate(in MinBroadcast broadcast)
                {
                    callback(broadcast.Value);
                },
                delegate(in MinBroadcast broadcast)
                {
                    return predicate(broadcast.Value);
                }
            );

            _subject!.Perform(new SyncMinOp(callback, predicate));
            return this;

            bool predicate(TValue? value) => condition?.Invoke(value) ?? true;
        }

        public Binding OnMax(Action<TValue?> callback, Func<TValue?, bool>? condition = null)
        {
            AddCallback(
                delegate(in MaxBroadcast broadcast)
                {
                    callback(broadcast.Value);
                },
                delegate(in MaxBroadcast broadcast)
                {
                    return predicate(broadcast.Value);
                }
            );

            _subject!.Perform(new SyncMaxOp(callback, predicate));
            return this;

            bool predicate(TValue? value) => condition?.Invoke(value) ?? true;
        }

        public Binding OnIsMin(Action<TValue?> callback, Func<TValue?, bool>? condition = null)
        {
            AddCallback(
                delegate(in IsMinBroadcast broadcast)
                {
                    callback(broadcast.Value);
                },
                delegate(in IsMinBroadcast broadcast)
                {
                    return predicate(broadcast.Value);
                }
            );

            _subject!.Perform(new SyncIsMinOp(callback, predicate));
            return this;

            bool predicate(TValue? value) => condition?.Invoke(value) ?? true;
        }

        public Binding OnIsMax(Action<TValue?> callback, Func<TValue?, bool>? condition = null)
        {
            AddCallback(
                delegate(in IsMaxBroadcast broadcast)
                {
                    callback(broadcast.Value);
                },
                delegate(in IsMaxBroadcast broadcast)
                {
                    return predicate(broadcast.Value);
                }
            );

            _subject!.Perform(new SyncIsMaxOp(callback, predicate));
            return this;

            bool predicate(TValue? value) => condition?.Invoke(value) ?? true;
        }

        public Binding On<TOperation>(Callback<TOperation> callback, Condition<TOperation>? condition = null)
            where TOperation : struct, IOperation<Stat<TValue>>
        {
            AddCallback(callback, condition);
            return this;
        }
    }
    #endregion
}

internal static class ComparerExtensions
{
    public static bool Equals<TValue>(this IComparer<TValue> comparer, TValue? a, TValue? b) => comparer.Compare(a!, b!) == 0;

    public static TValue? Min<TValue>(this IComparer<TValue> comparer, TValue? a, TValue? b) => comparer.Compare(a!, b!) < 0 ? a : b;

    public static TValue? Max<TValue>(this IComparer<TValue> comparer, TValue? a, TValue? b) => comparer.Compare(a!, b!) > 0 ? a : b;

    public static TValue? Clamp<TValue>(this IComparer<TValue> comparer, TValue? value, TValue? min, TValue? max) => comparer.Min(comparer.Max(value, min), max);
}

public static class StatExtensions
{
    extension<TValue>(IStat<TValue?> stat)
    {
        public bool IsMin => stat.Comparer.Equals(stat.Value, stat.Min);
        public bool IsMax => stat.Comparer.Equals(stat.Value, stat.Max);
    }

    extension<TValue>(Stat<TValue> stat)
    {
        public bool TrySet(TValue? value, out TValue? oldValue)
        {
            oldValue = stat.Value;
            stat.Value = value;
            return !stat.Comparer.Equals(oldValue, value);
        }

        public bool TrySetMin(TValue? min, out TValue? oldMin)
        {
            oldMin = stat.Min;
            stat.Min = min;
            return !stat.Comparer.Equals(oldMin, min);
        }

        public bool TrySetMax(TValue? max, out TValue? oldMax)
        {
            oldMax = stat.Max;
            stat.Max = max;
            return !stat.Comparer.Equals(oldMax, max);
        }
    }
}
