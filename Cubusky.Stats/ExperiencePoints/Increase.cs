using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.ExperiencePoints;

public readonly record struct Increase<TNumber>(TNumber Value)
    : IBuildingBlock<ExperiencePoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Increase()
    {
        ExperiencePoints<TNumber>.Set<Increase<TNumber>>(Callback);
    }

    public static void Callback(in ExperiencePoints<TNumber> experiencePoints, in Increase<TNumber> increase, in IBroadcaster<ExperiencePoints<TNumber>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(increase.Value, nameof(increase.Value));

        experiencePoints.TrySet(experiencePoints.Value + increase.Value, out var oldValue);
        broadcaster.Broadcast(new Increase<TNumber>(experiencePoints.Value - oldValue));

        throw new NotImplementedException("Should we start back from zero when experience points reaches Max?");
    }
}