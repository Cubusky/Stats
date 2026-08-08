using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.HitPoints;

public readonly record struct Damage<TNumber>(TNumber Value)
    : IBuildingBlock<HitPoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Damage()
    {
        HitPoints<TNumber>.Set<Damage<TNumber>>(Callback);
    }

    public static void Callback(in HitPoints<TNumber> hitPoints, in Damage<TNumber> damage, in IBroadcaster<HitPoints<TNumber>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(damage.Value, nameof(damage.Value));

        if (hitPoints.TrySet(hitPoints.Value - damage.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Damage<TNumber>(oldValue - hitPoints.Value));
        }
    }
}