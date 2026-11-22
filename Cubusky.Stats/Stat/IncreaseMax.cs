#if NET10_0_OR_GREATER
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;
using System.Numerics;

namespace Cubusky.Stats.Stat;

public readonly record struct IncreaseMax<TNumber>(TNumber Value, bool Recover = false) : IOperation<Stat<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static IncreaseMax()
    {
        OperationMap.Shared.Set<Stat<TNumber>, IncreaseMax<TNumber>, Stat<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(Stat<TNumber> stat, in IncreaseMax<TNumber> increaseMax, Stat<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(increaseMax.Value, nameof(increaseMax.Value));

        stat.Max += increaseMax.Value;
        broadcaster.Broadcast(increaseMax);

        if (increaseMax.Recover)
        {
            stat.Value += increaseMax.Value;
        }
    }
}
#endif
