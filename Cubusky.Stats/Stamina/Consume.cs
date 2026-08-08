using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.Stamina;

public readonly record struct Consume<TNumber>(TNumber Value)
    : IBuildingBlock<Stamina<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Consume()
    {
        Stamina<TNumber>.Set<Consume<TNumber>>(Callback);
    }

    public static void Callback(in Stamina<TNumber> stamina, in Consume<TNumber> consume, in IBroadcaster<Stamina<TNumber>> broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(consume.Value, nameof(consume.Value));

        if (stamina.TrySet(stamina.Value - consume.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Consume<TNumber>(oldValue - stamina.Value));
        }
    }
}
