#if NET10_0_OR_GREATER
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;
using System.Numerics;

namespace Cubusky.Stats.Stat;

public readonly record struct DecreaseMax<TNumber>(TNumber Value) : IOperation<Stat<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static DecreaseMax()
    {
        OperationMap.Shared.Set<Stat<TNumber>, DecreaseMax<TNumber>, Stat<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(Stat<TNumber> stat, in DecreaseMax<TNumber> decreaseMax, Stat<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(decreaseMax.Value, nameof(decreaseMax.Value));

        if (stat.TrySetMax(stat.Max - decreaseMax.Value, out var oldMax))
        {
            broadcaster.Broadcast(new DecreaseMax<TNumber>(oldMax - stat.Max));
        }
    }
}
#endif
