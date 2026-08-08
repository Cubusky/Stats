using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.Stamina;

public readonly record struct Recover<TNumber>(TNumber Value, bool IfExhausted = false)
    : IBuildingBlock<Stamina<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Recover()
    {
        Stamina<TNumber>.Set<Recover<TNumber>>(Callback);
    }

    public static void Callback(in Stamina<TNumber> stamina, in Recover<TNumber> recover, in IBroadcaster<Stamina<TNumber>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(recover.Value, nameof(recover.Value));
        if (stamina.IsExhausted && !recover.IfExhausted)
        {
            return;
        }

        if (stamina.TrySet(stamina.Value + recover.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Recover<TNumber>(stamina.Value - oldValue));
        }
    }
}