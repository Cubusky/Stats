using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.Stamina;

public readonly record struct Recover<TNumber>(TNumber Value, bool IfExhausted = false) : IOperation<Stamina<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Recover()
    {
        OperationMap.Shared.Add<Stamina<TNumber>, Recover<TNumber>, Stamina<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(Stamina<TNumber> stamina, in Recover<TNumber> recover, Stamina<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(recover.Value, nameof(recover.Value));
        if (stamina.IsExhausted && !recover.IfExhausted)
        {
            return;
        }

        if (stamina.TrySet(stamina.Value + recover.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Recover<TNumber>(stamina.Value - oldValue, recover.IfExhausted));
        }
    }
}
