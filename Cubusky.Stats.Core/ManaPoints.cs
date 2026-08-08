using Chickensoft.Collections;
using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface IManaPoints<TNumber> : IStat<TNumber>,
    IOperator<ManaPoints<TNumber>>,
    IAutoObject<ManaPoints<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new ManaPoints<TNumber>.Binding Bind();
}

public partial class ManaPoints<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    IManaPoints<TNumber>,
    IPerform<PerformOp<ManaPoints<TNumber>>>
    where TNumber : INumberBase<TNumber>
{
    #region Operation Callbacks
    private static readonly Blackboard _operationCallbacks = new();

    public static void Set<TOperation>(OperationCallback<ManaPoints<TNumber>, TOperation> callback)
        where TOperation : struct, IOperation<ManaPoints<TNumber>>
    {
        _operationCallbacks.Set(callback);
    }

    public static void Overwrite<TOperation>(OperationCallback<ManaPoints<TNumber>, TOperation> callback)
        where TOperation : struct, IOperation<ManaPoints<TNumber>>
    {
        _operationCallbacks.Overwrite(callback);
    }

    public static new bool Has<TOperation>()
        where TOperation : struct, IOperation<ManaPoints<TNumber>>
    {
        return _operationCallbacks.Has<OperationCallback<ManaPoints<TNumber>, TOperation>>();
    }

    public static new OperationCallback<ManaPoints<TNumber>, TOperation> Get<TOperation>()
        where TOperation : struct, IOperation<ManaPoints<TNumber>>
    {
        return _operationCallbacks.Get<OperationCallback<ManaPoints<TNumber>, TOperation>>();
    }
    #endregion

    #region Perform
    private Broadcaster OperationBroadcaster => field ??= new Broadcaster(_subject);
    private readonly BoxlessQueue<IOperation<ManaPoints<TNumber>>> _operationQueue = new();

    void IOperator<ManaPoints<TNumber>>.Perform<TOperation>(in TOperation operation)
    {
        _operationQueue.Enqueue(operation);
        _subject.Perform(new PerformOp<ManaPoints<TNumber>>(this, _operationCallbacks, OperationBroadcaster));
    }

    void IPerform<PerformOp<ManaPoints<TNumber>>>.Perform(in PerformOp<ManaPoints<TNumber>> op)
    {
        _operationQueue.Dequeue(op);
    }

    public new class Broadcaster : Stat<TNumber>.Broadcaster, IBroadcaster<ManaPoints<TNumber>>
    {
        internal Broadcaster(SyncSubject subject) : base(subject) { }

        void IBroadcaster<ManaPoints<TNumber>>.Broadcast<TBroadcast>(in TBroadcast broadcast)
        {
            _subject!.Broadcast(broadcast);
        }
    }
    #endregion

    #region Binding
    public new Binding Bind() => new(_subject);

    public new class Binding : Stat<TNumber>.Binding, IBinding<ManaPoints<TNumber>, Binding>
    {
        internal Binding(ISyncSubject subject) : base(subject) { }

        Binding IBinding<ManaPoints<TNumber>, Binding>.On<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition)
        {
            AddCallback(callback, condition);
            return this;
        }
    }
    #endregion
}

public static class ManaPointsExtensions
{
    extension<TNumber>(IManaPoints<TNumber> manaPoints)
        where TNumber : INumberBase<TNumber>
    {
        public bool IsDepleted => TNumber.IsZero(manaPoints.Value) || TNumber.IsNegative(manaPoints.Value);
    }
}

public static partial class BindingExtensions
{
    public static ManaPoints<TNumber>.Binding On<TNumber, TBroadcast>(this ManaPoints<TNumber>.Binding binding, Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
        where TNumber : INumberBase<TNumber>
        where TBroadcast : struct, IBroadcast<ManaPoints<TNumber>>
    {
        return ((IBinding<ManaPoints<TNumber>, ManaPoints<TNumber>.Binding>)binding).On(callback, condition);
    }
}