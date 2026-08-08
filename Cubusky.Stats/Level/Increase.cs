using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.Level;

public readonly record struct Increase<TInteger>(TInteger Value)
    : IBuildingBlock<Level<TInteger>>
    where TInteger : IBinaryInteger<TInteger>
{
    static Increase()
    {
        Level<TInteger>.Set<Increase<TInteger>>(Callback);
    }

    public static void Callback(in Level<TInteger> Level, in Increase<TInteger> increase, in IBroadcaster<Level<TInteger>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(increase.Value, nameof(increase.Value));

        if (Level.TrySet(Level.Value + increase.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Increase<TInteger>(Level.Value - oldValue));
        }
    }
}