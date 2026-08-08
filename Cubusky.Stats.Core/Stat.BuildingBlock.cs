using Chickensoft.Collections;
using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;

namespace Cubusky.Stats.Core;

public partial interface IStat<TValue>
    : IAutoObject<Stat<TValue>.Binding>,
    IOperator<Stat<TValue>>;

public partial class Stat<TValue>
    : IStat<TValue>,
    IPerform<PerformOp<Stat<TValue>>>
{
    #region Operation Callbacks
    private static readonly Blackboard _operationCallbacks = new();

    public static void Set<TOperation>(OperationCallback<Stat<TValue>, TOperation> callback)
        where TOperation : struct, IOperation<Stat<TValue>>
    {
        _operationCallbacks.Set(callback);
    }

    public static void Overwrite<TOperation>(OperationCallback<Stat<TValue>, TOperation> callback)
        where TOperation : struct, IOperation<Stat<TValue>>
    {
        _operationCallbacks.Overwrite(callback);
    }

    public static bool Has<TOperation>()
        where TOperation : struct, IOperation<Stat<TValue>>
    {
        return _operationCallbacks.Has<OperationCallback<Stat<TValue>, TOperation>>();
    }

    public static OperationCallback<Stat<TValue>, TOperation> Get<TOperation>()
        where TOperation : struct, IOperation<Stat<TValue>>
    {
        return _operationCallbacks.Get<OperationCallback<Stat<TValue>, TOperation>>();
    }
    #endregion

    #region Perform
    private Broadcaster OperationBroadcaster => field ??= new Broadcaster(_subject);
    private readonly BoxlessQueue<IOperation<Stat<TValue>>> _operationQueue = new();

    void IOperator<Stat<TValue>>.Perform<TOperation>(in TOperation operation)
    {
        _operationQueue.Enqueue(operation);
        _subject.Perform(new PerformOp<Stat<TValue>>(this, _operationCallbacks, OperationBroadcaster));
    }

    void IPerform<PerformOp<Stat<TValue>>>.Perform(in PerformOp<Stat<TValue>> op)
    {
        _operationQueue.Dequeue(op);
    }

    public class Broadcaster : IBroadcaster<Stat<TValue>>
    {
        protected SyncSubject? _subject;

        internal Broadcaster(SyncSubject subject)
        {
            _subject = subject;
        }

        void IBroadcaster<Stat<TValue>>.Broadcast<TBroadcast>(in TBroadcast broadcast)
        {
            _subject!.Broadcast(broadcast);
        }
    }
    #endregion

    #region Binding
    public partial class Binding : IBinding<Stat<TValue>, Binding>
    {
        Binding IBinding<Stat<TValue>, Binding>.On<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition)
        {
            AddCallback(callback, condition);
            return this;
        }
    }
    #endregion
}

public static partial class BindingExtensions
{
    public static Stat<TValue>.Binding On<TValue, TBroadcast>(this Stat<TValue>.Binding binding, Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
        where TBroadcast : struct, IBroadcast<Stat<TValue>>
    {
        return ((IBinding<Stat<TValue>, Stat<TValue>.Binding>)binding).On(callback, condition);
    }
}