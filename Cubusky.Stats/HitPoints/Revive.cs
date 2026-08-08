using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.HitPoints;

public readonly record struct Revive<TNumber>(TNumber Value)
    : IBuildingBlock<HitPoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Revive()
    {
        HitPoints<TNumber>.Set<Revive<TNumber>>(Callback);
    }

    public static void Callback(in HitPoints<TNumber> hitPoints, in Revive<TNumber> revive, in IBroadcaster<HitPoints<TNumber>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(revive.Value, nameof(revive.Value));
        if (!hitPoints.IsDead)
        {
            return;
        }

        hitPoints.TrySet(revive.Value, out _);
        broadcaster.Broadcast(new Revive<TNumber>(hitPoints.Value));
    }
}