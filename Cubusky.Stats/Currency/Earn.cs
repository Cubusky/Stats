using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.Currency;

public readonly record struct Earn<TNumber>(TNumber Value)
    : IBuildingBlock<Currency<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Earn()
    {
        Currency<TNumber>.Set<Earn<TNumber>>(Callback);
    }

    public static void Callback(in Currency<TNumber> currency, in Earn<TNumber> earn, in IBroadcaster<Currency<TNumber>> broadcaster)
    {
        if (currency.TrySet(earn.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Earn<TNumber>(currency.Value));
        }
    }
}
