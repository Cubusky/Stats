using Chickensoft.Collections;
using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface ILevel<TInteger> : IStat<TInteger>,
    IOperator<Level<TInteger>>,
    IAutoObject<Level<TInteger>.Binding>
    where TInteger : IBinaryInteger<TInteger>
{
    new Level<TInteger>.Binding Bind();
}

public class Level<TInteger>
(
    TInteger value,
    TInteger max,
    Comparer<TInteger>? comparer = null
)
    : Stat<TInteger>(value, TInteger.One, max, comparer),
    ILevel<TInteger>,
    IPerform<PerformOp<Level<TInteger>>>
    where TInteger : IBinaryInteger<TInteger>
{
    #region Operation Callbacks
    private static readonly Blackboard _operationCallbacks = new();

    public static void Set<TOperation>(OperationCallback<Level<TInteger>, TOperation> callback)
        where TOperation : struct, IOperation<Level<TInteger>>
    {
        _operationCallbacks.Set(callback);
    }

    public static void Overwrite<TOperation>(OperationCallback<Level<TInteger>, TOperation> callback)
        where TOperation : struct, IOperation<Level<TInteger>>
    {
        _operationCallbacks.Overwrite(callback);
    }

    public static new bool Has<TOperation>()
        where TOperation : struct, IOperation<Level<TInteger>>
    {
        return _operationCallbacks.Has<OperationCallback<Level<TInteger>, TOperation>>();
    }

    public static new OperationCallback<Level<TInteger>, TOperation> Get<TOperation>()
        where TOperation : struct, IOperation<Level<TInteger>>
    {
        return _operationCallbacks.Get<OperationCallback<Level<TInteger>, TOperation>>();
    }
    #endregion

    #region Perform
    private Broadcaster OperationBroadcaster => field ??= new Broadcaster(_subject);
    private readonly BoxlessQueue<IOperation<Level<TInteger>>> _operationQueue = new();

    void IOperator<Level<TInteger>>.Perform<TOperation>(in TOperation operation)
    {
        _operationQueue.Enqueue(operation);
        _subject.Perform(new PerformOp<Level<TInteger>>(this, _operationCallbacks, OperationBroadcaster));
    }

    void IPerform<PerformOp<Level<TInteger>>>.Perform(in PerformOp<Level<TInteger>> op)
    {
        _operationQueue.Dequeue(op);
    }

    public new class Broadcaster : Stat<TInteger>.Broadcaster, IBroadcaster<Level<TInteger>>
    {
        internal Broadcaster(SyncSubject subject) : base(subject) { }

        void IBroadcaster<Level<TInteger>>.Broadcast<TBroadcast>(in TBroadcast broadcast)
        {
            _subject!.Broadcast(broadcast);
        }
    }
    #endregion

    #region Binding
    public new Binding Bind() => new(_subject);

    public new class Binding : Stat<TInteger>.Binding, IBinding<Level<TInteger>, Binding>
    {
        internal Binding(ISyncSubject subject) : base(subject) { }

        Binding IBinding<Level<TInteger>, Binding>.On<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition)
        {
            AddCallback(callback, condition);
            return this;
        }
    }
    #endregion
}

public static partial class BindingExtensions
{
    public static Level<TInteger>.Binding On<TInteger, TBroadcast>(this Level<TInteger>.Binding binding, Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
        where TInteger : IBinaryInteger<TInteger>
        where TBroadcast : struct, IBroadcast<Level<TInteger>>
    {
        return ((IBinding<Level<TInteger>, Level<TInteger>.Binding>)binding).On(callback, condition);
    }
}