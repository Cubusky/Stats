using Cubusky.BuildingBlocks;
using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.Currency;

public readonly record struct Spend<TNumber>(TNumber Value)
    : IBuildingBlock<Currency<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Spend()
    {
        Currency<TNumber>.Set<Spend<TNumber>>(Callback);
    }

    public static void Callback(in Currency<TNumber> currency, in Spend<TNumber> spend, in IBroadcaster<Currency<TNumber>> broadcaster)
    {
        if (currency.TrySet(spend.Value, out _))
        {
            broadcaster.Broadcast(new Spend<TNumber>(currency.Value));
        }
    }
}
