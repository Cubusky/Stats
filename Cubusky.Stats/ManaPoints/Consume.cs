using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.ManaPoints;

public readonly record struct Consume<TNumber>(TNumber Value)
    : IBuildingBlock<ManaPoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Consume()
    {
        ManaPoints<TNumber>.Set<Consume<TNumber>>(Callback);
    }

    public static void Callback(in ManaPoints<TNumber> manapoints, in Consume<TNumber> consume, in IBroadcaster<ManaPoints<TNumber>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(consume.Value, nameof(consume.Value));

        if (manapoints.TrySet(manapoints.Value - consume.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Consume<TNumber>(oldValue - manapoints.Value));
        }
    }
}
