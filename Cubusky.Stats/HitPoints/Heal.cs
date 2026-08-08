using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.HitPoints;

public readonly record struct Heal<TNumber>(TNumber Value, bool IfDead = false)
    : IBuildingBlock<HitPoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Heal()
    {
        HitPoints<TNumber>.Set<Heal<TNumber>>(Callback);
    }

    public static void Callback(in HitPoints<TNumber> hitPoints, in Heal<TNumber> heal, in IBroadcaster<HitPoints<TNumber>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(heal.Value, nameof(heal.Value));
        if (hitPoints.IsDead && !heal.IfDead)
        {
            return;
        }

        if (hitPoints.TrySet(hitPoints.Value + heal.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Heal<TNumber>(hitPoints.Value - oldValue));
        }
    }
}