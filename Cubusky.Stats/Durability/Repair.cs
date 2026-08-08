using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.Durability;

public readonly record struct Repair<TNumber>(TNumber Value)
    : IBuildingBlock<Durability<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Repair()
    {
        Durability<TNumber>.Set<Repair<TNumber>>(Callback);
    }

    public static void Callback(in Durability<TNumber> durability, in Repair<TNumber> repair, in IBroadcaster<Durability<TNumber>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(repair.Value, nameof(repair.Value));

        if (durability.TrySet(durability.Value + repair.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Repair<TNumber>(durability.Value - oldValue));
        }
    }
}
