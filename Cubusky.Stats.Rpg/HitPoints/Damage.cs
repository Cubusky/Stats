using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.HitPoints;

public readonly record struct Damage<TNumber>(TNumber Value) : IOperation<HitPoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Damage()
    {
        OperationMap.Shared.Add<HitPoints<TNumber>, Damage<TNumber>, HitPoints<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(HitPoints<TNumber> hitPoints, in Damage<TNumber> damage, HitPoints<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(damage.Value, nameof(damage.Value));

        if (hitPoints.TrySet(hitPoints.Value - damage.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Damage<TNumber>(oldValue - hitPoints.Value));
        }
    }
}
