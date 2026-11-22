using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.ManaPoints;

public readonly record struct Consume<TNumber>(TNumber Value) : IOperation<ManaPoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Consume()
    {
        OperationMap.Shared.Add<ManaPoints<TNumber>, Consume<TNumber>, ManaPoints<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(ManaPoints<TNumber> manaPoints, in Consume<TNumber> consume, ManaPoints<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(consume.Value, nameof(consume.Value));

        if (manaPoints.TrySet(manaPoints.Value - consume.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Consume<TNumber>(oldValue - manaPoints.Value));
        }
    }
}
