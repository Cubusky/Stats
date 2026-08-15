using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.Level;

public readonly record struct Decrease<TInteger>(TInteger Value)
    : IBuildingBlock<Level<TInteger>>
    where TInteger : IBinaryInteger<TInteger>
{
    static Decrease()
    {
        Level<TInteger>.Set<Decrease<TInteger>>(Callback);
    }

    public static void Callback(in Level<TInteger> level, in Decrease<TInteger> decrease, in IBroadcaster<Level<TInteger>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(decrease.Value, nameof(decrease.Value));

        if (level.TrySet(level.Value - decrease.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Decrease<TInteger>(oldValue - level.Value));
        }
    }
}
