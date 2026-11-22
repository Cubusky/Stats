using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.HitPoints;

public readonly record struct Heal<TNumber>(TNumber Value, bool IfDead = false) : IOperation<HitPoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Heal()
    {
        OperationMap.Shared.Add<HitPoints<TNumber>, Heal<TNumber>, HitPoints<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(HitPoints<TNumber> hitPoints, in Heal<TNumber> heal, HitPoints<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(heal.Value, nameof(heal.Value));
        if (hitPoints.IsDead && !heal.IfDead)
        {
            return;
        }

        if (hitPoints.TrySet(hitPoints.Value + heal.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Heal<TNumber>(hitPoints.Value - oldValue, heal.IfDead));
        }
    }
}
