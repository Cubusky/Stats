using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.ManaPoints;

public readonly record struct Recover<TNumber>(TNumber Value, bool IfDepleted = false)
    : IBuildingBlock<ManaPoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Recover()
    {
        ManaPoints<TNumber>.Set<Recover<TNumber>>(Callback);
    }

    public static void Callback(in ManaPoints<TNumber> manapoints, in Recover<TNumber> recover, in IBroadcaster<ManaPoints<TNumber>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(recover.Value, nameof(recover.Value));
        if (manapoints.IsDepleted && !recover.IfDepleted)
        {
            return;
        }

        if (manapoints.TrySet(manapoints.Value + recover.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Recover<TNumber>(manapoints.Value - oldValue));
        }
    }
}