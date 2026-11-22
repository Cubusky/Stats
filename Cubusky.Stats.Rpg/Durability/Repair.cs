using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.Durability;

public readonly record struct Repair<TNumber>(TNumber Value) : IOperation<Durability<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Repair()
    {
        OperationMap.Shared.Add<Durability<TNumber>, Repair<TNumber>, Durability<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(Durability<TNumber> durability, in Repair<TNumber> repair, Durability<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(repair.Value, nameof(repair.Value));

        if (durability.TrySet(durability.Value + repair.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Repair<TNumber>(durability.Value - oldValue));
        }
    }
}
