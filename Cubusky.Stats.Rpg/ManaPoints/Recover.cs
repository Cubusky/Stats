using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.ManaPoints;

public readonly record struct Recover<TNumber>(TNumber Value, bool IfDepleted = false) : IOperation<ManaPoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Recover()
    {
        OperationMap.Shared.Add<ManaPoints<TNumber>, Recover<TNumber>, ManaPoints<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(ManaPoints<TNumber> manaPoints, in Recover<TNumber> recover, ManaPoints<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(recover.Value, nameof(recover.Value));
        if (manaPoints.IsDepleted && !recover.IfDepleted)
        {
            return;
        }

        if (manaPoints.TrySet(manaPoints.Value + recover.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Recover<TNumber>(manaPoints.Value - oldValue, recover.IfDepleted));
        }
    }
}
