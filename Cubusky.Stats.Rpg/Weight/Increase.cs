using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.Weight;

public readonly record struct Increase<TNumber>(TNumber Value) : IOperation<Weight<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Increase()
    {
        OperationMap.Shared.Add<Weight<TNumber>, Increase<TNumber>, Weight<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(Weight<TNumber> weight, in Increase<TNumber> increase, Weight<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(increase.Value, nameof(increase.Value));

        if (weight.TrySet(weight.Value + increase.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Increase<TNumber>(weight.Value - oldValue));
        }
    }
}
