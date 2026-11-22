using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.HitPoints;

public readonly record struct Revive<TNumber>(TNumber Value) : IOperation<HitPoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Revive()
    {
        OperationMap.Shared.Add<HitPoints<TNumber>, Revive<TNumber>, HitPoints<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(HitPoints<TNumber> hitPoints, in Revive<TNumber> revive, HitPoints<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(revive.Value, nameof(revive.Value));
        if (!hitPoints.IsDead)
        {
            return;
        }

        if (hitPoints.TrySet(revive.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Revive<TNumber>(hitPoints.Value - oldValue));
        }
    }
}
