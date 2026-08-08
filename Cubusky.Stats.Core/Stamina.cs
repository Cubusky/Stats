using Chickensoft.Collections;
using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface IStamina<TNumber> : IStat<TNumber>,
    IOperator<Stamina<TNumber>>,
    IAutoObject<Stamina<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new Stamina<TNumber>.Binding Bind();
}

public partial class Stamina<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    IStamina<TNumber>,
    IPerform<PerformOp<Stamina<TNumber>>>
    where TNumber : INumberBase<TNumber>
{
    #region Operation Callbacks
    private static readonly Blackboard _operationCallbacks = new();

    public static void Set<TOperation>(OperationCallback<Stamina<TNumber>, TOperation> callback)
        where TOperation : struct, IOperation<Stamina<TNumber>>
    {
        _operationCallbacks.Set(callback);
    }

    public static void Overwrite<TOperation>(OperationCallback<Stamina<TNumber>, TOperation> callback)
        where TOperation : struct, IOperation<Stamina<TNumber>>
    {
        _operationCallbacks.Overwrite(callback);
    }

    public static new bool Has<TOperation>()
        where TOperation : struct, IOperation<Stamina<TNumber>>
    {
        return _operationCallbacks.Has<OperationCallback<Stamina<TNumber>, TOperation>>();
    }

    public static new OperationCallback<Stamina<TNumber>, TOperation> Get<TOperation>()
        where TOperation : struct, IOperation<Stamina<TNumber>>
    {
        return _operationCallbacks.Get<OperationCallback<Stamina<TNumber>, TOperation>>();
    }
    #endregion

    #region Perform
    private Broadcaster OperationBroadcaster => field ??= new Broadcaster(_subject);
    private readonly BoxlessQueue<IOperation<Stamina<TNumber>>> _operationQueue = new();

    void IOperator<Stamina<TNumber>>.Perform<TOperation>(in TOperation operation)
    {
        _operationQueue.Enqueue(operation);
        _subject.Perform(new PerformOp<Stamina<TNumber>>(this, _operationCallbacks, OperationBroadcaster));
    }

    void IPerform<PerformOp<Stamina<TNumber>>>.Perform(in PerformOp<Stamina<TNumber>> op)
    {
        _operationQueue.Dequeue(op);
    }

    public new class Broadcaster : Stat<TNumber>.Broadcaster, IBroadcaster<Stamina<TNumber>>
    {
        internal Broadcaster(SyncSubject subject) : base(subject) { }

        void IBroadcaster<Stamina<TNumber>>.Broadcast<TBroadcast>(in TBroadcast broadcast)
        {
            _subject!.Broadcast(broadcast);
        }
    }
    #endregion

    #region Binding
    public new Binding Bind() => new(_subject);

    public new class Binding : Stat<TNumber>.Binding, IBinding<Stamina<TNumber>, Binding>
    {
        internal Binding(ISyncSubject subject) : base(subject) { }

        Binding IBinding<Stamina<TNumber>, Binding>.On<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition)
        {
            AddCallback(callback, condition);
            return this;
        }
    }
    #endregion
}

public static class StaminaExtensions
{
    extension<TNumber>(IStamina<TNumber> stamina)
        where TNumber : INumberBase<TNumber>
    {
        public bool IsExhausted => TNumber.IsZero(stamina.Value) || TNumber.IsNegative(stamina.Value);
    }
}

public static partial class BindingExtensions
{
    public static Stamina<TNumber>.Binding On<TNumber, TBroadcast>(this Stamina<TNumber>.Binding binding, Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
        where TNumber : INumberBase<TNumber>
        where TBroadcast : struct, IBroadcast<Stamina<TNumber>>
    {
        return ((IBinding<Stamina<TNumber>, Stamina<TNumber>.Binding>)binding).On(callback, condition);
    }
}