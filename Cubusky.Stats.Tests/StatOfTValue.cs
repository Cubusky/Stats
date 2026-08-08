//using Chickensoft.Sync;
//using Chickensoft.Sync.Primitives;

//namespace Cubusky.Stats.Tests;

//public interface IStat<TValue>
//    : IAutoObject<Stat<TValue>.Binding>,
//    IDisposable
//{
//    TValue Value { get; }
//    TValue Min { get; }
//    TValue Max { get; }
//    IComparer<TValue> Comparer { get; }
//}

//public class Stat<TValue> : Stat,
//    IStat<TValue>,
//    IPerform<Stat<TValue>.SetValueOp>,
//    IPerform<Stat<TValue>.SetMinOp>,
//    IPerform<Stat<TValue>.SetMaxOp>
//{
//    public Stat(TValue value, TValue min, TValue max, IComparer<TValue>? comparer = null)
//    {
//        Comparer = comparer ?? Comparer<TValue>.Default;

//        if (Comparer.Compare(value, min) < 0)
//        {
//            throw new ArgumentOutOfRangeException(nameof(value), value, $"Value {value} cannot be less than min value {min} on {GetType()} initialization.");
//        }

//        if (Comparer.Compare(value, max) > 0)
//        {
//            throw new ArgumentOutOfRangeException(nameof(value), value, $"Value {value} cannot be greater than max value {max} on {GetType()} initialization.");
//        }

//        if (Comparer.Compare(min, max) > 0)
//        {
//            throw new ArgumentOutOfRangeException(nameof(min), min, $"Min value {min} cannot be greater than max value {max} on {GetType()} initialization.");
//        }

//        _value = value;
//        _min = min;
//        _max = max;
//    }

//    #region Values
//    private TValue _value;
//    public TValue Value
//    {
//        get => _value;
//        set => _subject.Perform(new SetValueOp(value));
//    }

//    private TValue _min;
//    public TValue Min
//    {
//        get => _min;
//        set => _subject.Perform(new SetMinOp(value));
//    }

//    private TValue _max;
//    public TValue Max
//    {
//        get => _max;
//        set => _subject.Perform(new SetMaxOp(value));
//    }

//    public IComparer<TValue> Comparer { get; }
//    #endregion

//    #region Perform
//    //public interface IPerform<out TSelf>
//    //    where TSelf : struct
//    //{
//    //    TSelf Perform(in Stat<TValue> stat);
//    //}

//    //public void Perform<TOp>(in TOp op)
//    //    where TOp : struct, ISelf<TOp>
//    //{
//    //    _subject.Perform(op);
//    //}

//    //public static void Perform<TStat, TOp>(TStat stat, in TOp op)
//    //    where TStat : IStat
//    //    where TOp : struct, IPerform<TStat, TOp>
//    //{
//    //    stat.Subject.Perform(op);
//    //    //_subject.Perform(op);
//    //}

//    private readonly record struct SetValueOp(TValue Value);
//    private readonly record struct SetMinOp(TValue Value);
//    private readonly record struct SetMaxOp(TValue Value);

//    void IPerform<SetValueOp>.Perform(in SetValueOp op)
//    {
//        var clamped = Comparer.Clamp(op.Value, Min, Max);
//        if (Comparer.Equals(_value, clamped))
//        {
//            return;
//        }

//        _subject.Broadcast(new ValueBroadcast(_value = clamped));

//        if (Comparer.Equals(_value, Min))
//        {
//            _subject.Broadcast(new IsMinBroadcast(_value));
//        }

//        if (Comparer.Equals(_value, Max))
//        {
//            _subject.Broadcast(new IsMaxBroadcast(_value));
//        }
//    }

//    void IPerform<SetMinOp>.Perform(in SetMinOp op)
//    {
//        var min = Comparer.Min(op.Value, Max);
//        if (Comparer.Equals(_min, min))
//        {
//            return;
//        }

//        _subject.Broadcast(new MinBroadcast(_min = min));
//        _subject.Perform(new SetValueOp(_value));
//    }

//    void IPerform<SetMaxOp>.Perform(in SetMaxOp op)
//    {
//        var max = Comparer.Max(op.Value, Min);
//        if (Comparer.Equals(_max, max))
//        {
//            return;
//        }

//        _subject.Broadcast(new MaxBroadcast(_max = max));
//        _subject.Perform(new SetValueOp(_value));
//    }

//    protected override void Perform<TOp>(in TOp op) => Perform(this, op);

//    //protected static void PerformOp<TStat, TOp>(TStat stat, in TOp op)
//    //    where TStat : IStat
//    //    where TOp : struct
//    //{
//    //    if (op is IPerform<TStat, TOp> statOp)
//    //    {
//    //        var broadcast = statOp.Perform(stat);
//    //        stat.Subject.Broadcast(in broadcast);
//    //    }
//    //}

//    //void IPerformAnyOperation.Perform<TOp>(in TOp op)
//    //{
//    //    PerformOp(this, op);

//    //    //if (op is IPerform<TOp> statOp)
//    //    //{
//    //    //    var broadcast = statOp.Perform(this);
//    //    //    _subject.Broadcast(in broadcast);
//    //    //}
//    //}
//    #endregion

//    #region Broadcast
//    private readonly record struct ValueBroadcast(TValue Value) : IStatBroadcast<TValue>;
//    private readonly record struct MinBroadcast(TValue Value) : IStatBroadcast<TValue>;
//    private readonly record struct MaxBroadcast(TValue Value) : IStatBroadcast<TValue>;
//    private readonly record struct IsMinBroadcast(TValue Value) : IStatBroadcast<TValue>;
//    private readonly record struct IsMaxBroadcast(TValue Value) : IStatBroadcast<TValue>;
//    #endregion

//    #region Binding
//    public Binding Bind() => new(_subject);
//    public void ClearBindings() => _subject.ClearBindings();
//    public void Dispose()
//    {
//        GC.SuppressFinalize(this);
//        _subject.Dispose();
//    }

//    public class Binding : SyncBinding
//    {
//        internal Binding(SyncSubject subject) : base(subject) { }

//        #region Value Broadcasts
//        public Binding OnValue(Action callback, Func<bool>? condition = null)
//            => On<ValueBroadcast>(callback, condition);

//        public Binding OnValue(Action<TValue> callback, Func<TValue, bool>? condition = null)
//            => On<ValueBroadcast>(callback, condition);

//        public Binding OnMin(Action callback, Func<bool>? condition = null)
//            => On<MinBroadcast>(callback, condition);

//        public Binding OnMin(Action<TValue> callback, Func<TValue, bool>? condition = null)
//            => On<MinBroadcast>(callback, condition);

//        public Binding OnMax(Action callback, Func<bool>? condition = null)
//            => On<MaxBroadcast>(callback, condition);

//        public Binding OnMax(Action<TValue> callback, Func<TValue, bool>? condition = null)
//            => On<MaxBroadcast>(callback, condition);

//        public Binding OnIsMin(Action callback, Func<bool>? condition = null)
//            => On<IsMinBroadcast>(callback, condition);

//        public Binding OnIsMin(Action<TValue> callback, Func<TValue, bool>? condition = null)
//            => On<IsMinBroadcast>(callback, condition);

//        public Binding OnIsMax(Action callback, Func<bool>? condition = null)
//            => On<IsMaxBroadcast>(callback, condition);

//        public Binding OnIsMax(Action<TValue> callback, Func<TValue, bool>? condition = null)
//            => On<IsMaxBroadcast>(callback, condition);
//        #endregion

//        #region Generic Broadcasts
//        public Binding On<TBroadcast>(Callback<TBroadcast> broadcast, Condition<TBroadcast>? condition = null)
//            where TBroadcast : struct
//        {
//            AddCallback(broadcast, condition);
//            return this;
//        }

//        public Binding On<TBroadcast>(Action<TValue> callback, Func<TValue, bool>? condition = null)
//            where TBroadcast : struct, IStatBroadcast<TValue>
//        {
//            AddCallback
//            (
//                delegate (in TBroadcast broadcast) { callback(broadcast.Value); },
//                delegate (in TBroadcast broadcast) { return predicate(broadcast.Value); }
//            );

//            bool predicate(in TValue value) => condition?.Invoke(value) ?? true;
//            return this;
//        }

//        public Binding On<TBroadcast>(Action callback, Func<bool>? condition = null)
//            where TBroadcast : struct
//        {
//            AddCallback
//            (
//                delegate (in TBroadcast broadcast) { callback(); },
//                delegate (in TBroadcast broadcast) { return predicate(); }
//            );

//            bool predicate() => condition?.Invoke() ?? true;
//            return this;
//        }
//        #endregion
//    }
//    #endregion
//}

//internal static class ComparerExtensions
//{
//    public static bool Equals<TValue>(this IComparer<TValue> comparer, TValue a, TValue b) => comparer.Compare(a, b) == 0;

//    public static TValue Min<TValue>(this IComparer<TValue> comparer, TValue a, TValue b) => comparer.Compare(a, b) < 0 ? a : b;
//    public static TValue Max<TValue>(this IComparer<TValue> comparer, TValue a, TValue b) => comparer.Compare(a, b) > 0 ? a : b;
//    public static TValue Clamp<TValue>(this IComparer<TValue> comparer, TValue value, TValue min, TValue max) => comparer.Min(comparer.Max(value, min), max);
//}
