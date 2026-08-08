using Chickensoft.Collections;
using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface IExperiencePoints<TNumber> : IStat<TNumber>,
    IOperator<ExperiencePoints<TNumber>>,
    IAutoObject<ExperiencePoints<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new ExperiencePoints<TNumber>.Binding Bind();
}

public partial class ExperiencePoints<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    IExperiencePoints<TNumber>,
    IPerform<PerformOp<ExperiencePoints<TNumber>>>
    where TNumber : INumberBase<TNumber>
{
    #region Operation Callbacks
    private static readonly Blackboard _operationCallbacks = new();

    public static void Set<TOperation>(OperationCallback<ExperiencePoints<TNumber>, TOperation> callback)
        where TOperation : struct, IOperation<ExperiencePoints<TNumber>>
    {
        _operationCallbacks.Set(callback);
    }

    public static void Overwrite<TOperation>(OperationCallback<ExperiencePoints<TNumber>, TOperation> callback)
        where TOperation : struct, IOperation<ExperiencePoints<TNumber>>
    {
        _operationCallbacks.Overwrite(callback);
    }

    public static new bool Has<TOperation>()
        where TOperation : struct, IOperation<ExperiencePoints<TNumber>>
    {
        return _operationCallbacks.Has<OperationCallback<ExperiencePoints<TNumber>, TOperation>>();
    }

    public static new OperationCallback<ExperiencePoints<TNumber>, TOperation> Get<TOperation>()
        where TOperation : struct, IOperation<ExperiencePoints<TNumber>>
    {
        return _operationCallbacks.Get<OperationCallback<ExperiencePoints<TNumber>, TOperation>>();
    }
    #endregion

    #region Perform
    private Broadcaster OperationBroadcaster => field ??= new Broadcaster(_subject);
    private readonly BoxlessQueue<IOperation<ExperiencePoints<TNumber>>> _operationQueue = new();

    void IOperator<ExperiencePoints<TNumber>>.Perform<TOperation>(in TOperation operation)
    {
        _operationQueue.Enqueue(operation);
        _subject.Perform(new PerformOp<ExperiencePoints<TNumber>>(this, _operationCallbacks, OperationBroadcaster));
    }

    void IPerform<PerformOp<ExperiencePoints<TNumber>>>.Perform(in PerformOp<ExperiencePoints<TNumber>> op)
    {
        _operationQueue.Dequeue(op);
    }

    public new class Broadcaster : Stat<TNumber>.Broadcaster, IBroadcaster<ExperiencePoints<TNumber>>
    {
        internal Broadcaster(SyncSubject subject) : base(subject) { }

        void IBroadcaster<ExperiencePoints<TNumber>>.Broadcast<TBroadcast>(in TBroadcast broadcast)
        {
            _subject!.Broadcast(broadcast);
        }
    }
    #endregion

    #region Binding
    public new Binding Bind() => new(_subject);

    public new class Binding : Stat<TNumber>.Binding, IBinding<ExperiencePoints<TNumber>, Binding>
    {
        internal Binding(ISyncSubject subject) : base(subject) { }

        Binding IBinding<ExperiencePoints<TNumber>, Binding>.On<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition)
        {
            AddCallback(callback, condition);
            return this;
        }
    }
    #endregion
}

public static partial class BindingExtensions
{
    public static ExperiencePoints<TNumber>.Binding On<TNumber, TBroadcast>(this ExperiencePoints<TNumber>.Binding binding, Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
        where TNumber : INumberBase<TNumber>
        where TBroadcast : struct, IBroadcast<ExperiencePoints<TNumber>>
    {
        return ((IBinding<ExperiencePoints<TNumber>, ExperiencePoints<TNumber>.Binding>)binding).On(callback, condition);
    }
}