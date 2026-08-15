using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.Stat;

public readonly record struct IncreaseMax<TNumber>(TNumber Value, bool Recover = false)
    : IBuildingBlock<Stat<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static IncreaseMax()
    {
        Stat<TNumber>.Set<IncreaseMax<TNumber>>(Callback);
    }

    public static void Callback(in Stat<TNumber> stat, in IncreaseMax<TNumber> increaseMax, in IBroadcaster<Stat<TNumber>> broadcaster)
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