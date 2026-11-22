using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.Level;

public readonly record struct Increase<TInteger>(TInteger Value) : IOperation<Level<TInteger>>
    where TInteger : IBinaryInteger<TInteger>
{
    static Increase()
    {
        OperationMap.Shared.Add<Level<TInteger>, Increase<TInteger>, Level<TInteger>.Broadcaster>(Callback);
    }

    public static void Callback(Level<TInteger> level, in Increase<TInteger> increase, Level<TInteger>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(increase.Value, nameof(increase.Value));

        if (level.TrySet(level.Value + increase.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Increase<TInteger>(level.Value - oldValue));
        }
    }
}
