using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.Stamina;

public readonly record struct Consume<TNumber>(TNumber Value) : IOperation<Stamina<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Consume()
    {
        OperationMap.Shared.Add<Stamina<TNumber>, Consume<TNumber>, Stamina<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(Stamina<TNumber> stamina, in Consume<TNumber> consume, Stamina<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(consume.Value, nameof(consume.Value));

        if (stamina.TrySet(stamina.Value - consume.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Consume<TNumber>(oldValue - stamina.Value));
        }
    }
}
