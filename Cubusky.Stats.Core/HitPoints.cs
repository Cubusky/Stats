using Chickensoft.Collections;
using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface IHitPoints<TNumber> : IStat<TNumber>,
    IOperator<HitPoints<TNumber>>,
    IAutoObject<HitPoints<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new HitPoints<TNumber>.Binding Bind();
}

public partial class HitPoints<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    IHitPoints<TNumber>,
    IPerform<PerformOp<HitPoints<TNumber>>>
    where TNumber : INumberBase<TNumber>
{
    #region Operation Callbacks
    private static readonly Blackboard _operationCallbacks = new();

    public static void Set<TOperation>(OperationCallback<HitPoints<TNumber>, TOperation> callback)
        where TOperation : struct, IOperation<HitPoints<TNumber>>
    {
        _operationCallbacks.Set(callback);
    }

    public static void Overwrite<TOperation>(OperationCallback<HitPoints<TNumber>, TOperation> callback)
        where TOperation : struct, IOperation<HitPoints<TNumber>>
    {
        _operationCallbacks.Overwrite(callback);
    }

    public static new bool Has<TOperation>()
        where TOperation : struct, IOperation<HitPoints<TNumber>>
    {
        return _operationCallbacks.Has<OperationCallback<HitPoints<TNumber>, TOperation>>();
    }

    public static new OperationCallback<HitPoints<TNumber>, TOperation> Get<TOperation>()
        where TOperation : struct, IOperation<HitPoints<TNumber>>
    {
        return _operationCallbacks.Get<OperationCallback<HitPoints<TNumber>, TOperation>>();
    }
    #endregion

    #region Perform
    private Broadcaster OperationBroadcaster => field ??= new Broadcaster(_subject);
    private readonly BoxlessQueue<IOperation<HitPoints<TNumber>>> _operationQueue = new();

    void IOperator<HitPoints<TNumber>>.Perform<TOperation>(in TOperation operation)
    {
        _operationQueue.Enqueue(operation);
        _subject.Perform(new PerformOp<HitPoints<TNumber>>(this, _operationCallbacks, OperationBroadcaster));
    }

    void IPerform<PerformOp<HitPoints<TNumber>>>.Perform(in PerformOp<HitPoints<TNumber>> op)
    {
        _operationQueue.Dequeue(op);
    }

    public new class Broadcaster : Stat<TNumber>.Broadcaster, IBroadcaster<HitPoints<TNumber>>
    {
        internal Broadcaster(SyncSubject subject) : base(subject) { }

        void IBroadcaster<HitPoints<TNumber>>.Broadcast<TBroadcast>(in TBroadcast broadcast)
        {
            _subject!.Broadcast(broadcast);
        }
    }
    #endregion

    #region Binding
    public new Binding Bind() => new(_subject);

    public new class Binding : Stat<TNumber>.Binding, IBinding<HitPoints<TNumber>, Binding>
    {
        internal Binding(ISyncSubject subject) : base(subject) { }

        Binding IBinding<HitPoints<TNumber>, Binding>.On<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition)
        {
            AddCallback(callback, condition);
            return this;
        }
    }
    #endregion
}

public static class HitPointsExtensions
{
    extension<TNumber>(IHitPoints<TNumber> hitPoints)
        where TNumber : INumberBase<TNumber>
    {
        public bool IsDead => TNumber.IsZero(hitPoints.Value) || TNumber.IsNegative(hitPoints.Value);
    }
}

public static partial class BindingExtensions
{
    public static HitPoints<TNumber>.Binding On<TNumber, TBroadcast>(this HitPoints<TNumber>.Binding binding, Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
        where TNumber : INumberBase<TNumber>
        where TBroadcast : struct, IBroadcast<HitPoints<TNumber>>
    {
        return ((IBinding<HitPoints<TNumber>, HitPoints<TNumber>.Binding>)binding).On(callback, condition);
    }
}