using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.ExperiencePoints;

public readonly record struct Increase<TNumber>(TNumber Value, int Loops = 0) : IOperation<ExperiencePoints<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Increase()
    {
        OperationMap.Shared.Add<ExperiencePoints<TNumber>, Increase<TNumber>, ExperiencePoints<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(ExperiencePoints<TNumber> experiencePoints, in Increase<TNumber> increase, ExperiencePoints<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(increase.Value, nameof(increase.Value));

        var (min, max) = (experiencePoints.Min, experiencePoints.Max);
        var minMaxDelta = max - min;
        var total = experiencePoints.Value + increase.Value;

        // Saturating: an enormous loop count simply exceeds increase.Loops instead of throwing.
        var loops = int.CreateSaturating((total - min) / minMaxDelta); // Account for cases where min is not zero
        if (loops > increase.Loops)
        {
            if (experiencePoints.TrySet(max, out var oldValue))
            {
                total = max - oldValue;
                broadcaster.Broadcast(new Increase<TNumber>(increase.Loops == 0 ? total : total + minMaxDelta * TNumber.CreateChecked(increase.Loops), increase.Loops));
            }
        }
        else
        {
            experiencePoints.Value = loops == 0 ? total : total - minMaxDelta * TNumber.CreateChecked(loops);
            broadcaster.Broadcast(increase);
        }
    }
}
