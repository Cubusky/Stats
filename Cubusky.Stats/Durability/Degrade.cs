using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.Durability;

public readonly record struct Degrade<TNumber>(TNumber Value)
    : IBuildingBlock<Durability<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Degrade()
    {
        Durability<TNumber>.Set<Degrade<TNumber>>(Callback);
    }

    public static void Callback(in Durability<TNumber> durability, in Degrade<TNumber> degrade, in IBroadcaster<Durability<TNumber>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(degrade.Value, nameof(degrade.Value));

        if (durability.TrySet(durability.Value - degrade.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Degrade<TNumber>(oldValue - durability.Value));
        }
    }
}
