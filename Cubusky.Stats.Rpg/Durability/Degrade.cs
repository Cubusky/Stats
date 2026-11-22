using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.Durability;

public readonly record struct Degrade<TNumber>(TNumber Value) : IOperation<Durability<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Degrade()
    {
        OperationMap.Shared.Add<Durability<TNumber>, Degrade<TNumber>, Durability<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(Durability<TNumber> durability, in Degrade<TNumber> degrade, Durability<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(degrade.Value, nameof(degrade.Value));

        if (durability.TrySet(durability.Value - degrade.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Degrade<TNumber>(oldValue - durability.Value));
        }
    }
}
