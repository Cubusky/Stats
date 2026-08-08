//using Chickensoft.Sync;
//using Chickensoft.Sync.Primitives;

//namespace Cubusky.Stats.Tests;

//public interface IPerform<in TStat, out TSelf>
//    where TStat : Stat
//    where TSelf : struct
//{
//    TSelf Perform(TStat stat);
//}

//public abstract class Stat : IPerformAnyOperation, IAutoObject<Stat.Binding>
//{
//    internal protected readonly SyncSubject _subject;

//    public Stat()
//    {
//        _subject = new(this);
//    }

//    protected static void Perform<TStat, TOp>(in TStat stat, in TOp op)
//        where TStat : Stat
//        where TOp : struct
//    {
//        if (op is IPerform<TStat, TOp> typedOp)
//        {
//            var broadcast = typedOp.Perform(stat);
//            stat._subject.Broadcast(broadcast);
//        }
//    }

//    protected abstract void Perform<TOp>(in TOp op)
//        where TOp : struct;

//    void IPerformAnyOperation.Perform<TOp>(in TOp op)
//    {
//        Perform(op);
//    }

//    public Binding Bind() => new(_subject);
//    public void ClearBindings() => _subject.ClearBindings();
//    public void Dispose()
//    {
//        GC.SuppressFinalize(this);
//        _subject.Dispose();
//    }

//    public class Binding : SyncBinding
//    {
//        internal Binding(ISyncSubject subject) : base(subject) { }

//        public Binding On<TStat, TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
//            where TStat : Stat
//            where TBroadcast : struct, IPerform<TStat, TBroadcast>
//        {
//            AddCallback(callback, condition);
//            return this;
//        }
//    }
//}

//public static class Stats
//{
//    public static void Perform<TStat, TOp>(this TStat stat, in TOp op)
//        where TStat : Stat
//        where TOp : struct, IPerform<TStat, TOp>
//    {
//        stat._subject.Perform(op);
//    }
//}
