using Chickensoft.Collections;
using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface IDurability<TNumber> : IStat<TNumber>,
    IOperator<Durability<TNumber>>,
    IAutoObject<Durability<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new Durability<TNumber>.Binding Bind();
}

public class Durability<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    IDurability<TNumber>,
    IPerform<PerformOp<Durability<TNumber>>>
    where TNumber : INumberBase<TNumber>
{
    #region Operation Callbacks
    private static readonly Blackboard _operationCallbacks = new();

    public static void Set<TOperation>(OperationCallback<Durability<TNumber>, TOperation> callback)
        where TOperation : struct, IOperation<Durability<TNumber>>
    {
        _operationCallbacks.Set(callback);
    }

    public static void Overwrite<TOperation>(OperationCallback<Durability<TNumber>, TOperation> callback)
        where TOperation : struct, IOperation<Durability<TNumber>>
    {
        _operationCallbacks.Overwrite(callback);
    }

    public static new bool Has<TOperation>()
        where TOperation : struct, IOperation<Durability<TNumber>>
    {
        return _operationCallbacks.Has<OperationCallback<Durability<TNumber>, TOperation>>();
    }

    public static new OperationCallback<Durability<TNumber>, TOperation> Get<TOperation>()
        where TOperation : struct, IOperation<Durability<TNumber>>
    {
        return _operationCallbacks.Get<OperationCallback<Durability<TNumber>, TOperation>>();
    }
    #endregion

    #region Perform
    private Broadcaster OperationBroadcaster => field ??= new Broadcaster(_subject);
    private readonly BoxlessQueue<IOperation<Durability<TNumber>>> _operationQueue = new();

    void IOperator<Durability<TNumber>>.Perform<TOperation>(in TOperation operation)
    {
        _operationQueue.Enqueue(operation);
        _subject.Perform(new PerformOp<Durability<TNumber>>(this, _operationCallbacks, OperationBroadcaster));
    }

    void IPerform<PerformOp<Durability<TNumber>>>.Perform(in PerformOp<Durability<TNumber>> op)
    {
        _operationQueue.Dequeue(op);
    }

    public new class Broadcaster : Stat<TNumber>.Broadcaster, IBroadcaster<Durability<TNumber>>
    {
        internal Broadcaster(SyncSubject subject) : base(subject) { }

        void IBroadcaster<Durability<TNumber>>.Broadcast<TBroadcast>(in TBroadcast broadcast)
        {
            _subject!.Broadcast(broadcast);
        }
    }
    #endregion

    #region Binding
    public new Binding Bind() => new(_subject);

    public new class Binding : Stat<TNumber>.Binding, IBinding<Durability<TNumber>, Binding>
    {
        internal Binding(ISyncSubject subject) : base(subject) { }

        Binding IBinding<Durability<TNumber>, Binding>.On<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition)
        {
            AddCallback(callback, condition);
            return this;
        }
    }
    #endregion
}

public static partial class BindingExtensions
{
    public static Durability<TNumber>.Binding On<TNumber, TBroadcast>(this Durability<TNumber>.Binding binding, Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
        where TNumber : INumberBase<TNumber>
        where TBroadcast : struct, IBroadcast<Durability<TNumber>>
    {
        return ((IBinding<Durability<TNumber>, Durability<TNumber>.Binding>)binding).On(callback, condition);
    }
}