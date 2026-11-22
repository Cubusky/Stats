using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.Level;

public readonly record struct Decrease<TInteger>(TInteger Value) : IOperation<Level<TInteger>>
    where TInteger : IBinaryInteger<TInteger>
{
    static Decrease()
    {
        OperationMap.Shared.Add<Level<TInteger>, Decrease<TInteger>, Level<TInteger>.Broadcaster>(Callback);
    }

    public static void Callback(Level<TInteger> level, in Decrease<TInteger> decrease, Level<TInteger>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(decrease.Value, nameof(decrease.Value));

        if (level.TrySet(level.Value - decrease.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Decrease<TInteger>(oldValue - level.Value));
        }
    }
}
