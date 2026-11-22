using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.Weight;

public readonly record struct Decrease<TNumber>(TNumber Value) : IOperation<Weight<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Decrease()
    {
        OperationMap.Shared.Add<Weight<TNumber>, Decrease<TNumber>, Weight<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(Weight<TNumber> weight, in Decrease<TNumber> decrease, Weight<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(decrease.Value, nameof(decrease.Value));

        if (weight.TrySet(weight.Value - decrease.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Decrease<TNumber>(oldValue - weight.Value));
        }
    }
}
