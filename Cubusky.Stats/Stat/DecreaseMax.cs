using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.Stat;

public readonly record struct DecreaseMax<TNumber>(TNumber Value)
    : IBuildingBlock<Stat<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static DecreaseMax()
    {
        Stat<TNumber>.Set<DecreaseMax<TNumber>>(Callback);
    }

    public static void Callback(in Stat<TNumber> stat, in DecreaseMax<TNumber> decreaseMax, in IBroadcaster<Stat<TNumber>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(decreaseMax.Value, nameof(decreaseMax.Value));

        if (stat.TrySetMax(stat.Max - decreaseMax.Value, out var oldMax))
        {
            broadcaster.Broadcast(new DecreaseMax<TNumber>(oldMax - stat.Max));
        }
    }
}