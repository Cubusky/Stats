using Chickensoft.Collections;

namespace Cubusky.BuildingBlocks;

public delegate void OperationCallback<TConformance, TOperation>(in TConformance owner, in TOperation operation, in IBroadcaster<TConformance> broadcaster)
    where TOperation : struct, IOperation<TConformance>;

public readonly record struct PerformOp<TConformance>(TConformance Owner, IReadOnlyBlackboard OperationCallbacks, IBroadcaster<TConformance> Broadcaster)
    : IBoxlessValueHandler<IOperation<TConformance>>
{
    void IBoxlessValueHandler<IOperation<TConformance>>.HandleValue<TValue>(in TValue value)
    {
        var operationCallback = OperationCallbacks.Get<OperationCallback<TConformance, TValue>>();
        operationCallback(Owner, value, Broadcaster);
    }
}
