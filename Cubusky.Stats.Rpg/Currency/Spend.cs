using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.Currency;

public readonly record struct Spend<TNumber>(TNumber Value) : IOperation<Currency<TNumber>>
    where TNumber : INumber<TNumber>
{
    static Spend()
    {
        OperationMap.Shared.Add<Currency<TNumber>, Spend<TNumber>, Currency<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(Currency<TNumber> currency, in Spend<TNumber> spend, Currency<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(spend.Value, nameof(spend.Value));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(spend.Value, currency.Value, nameof(spend.Value));

        currency.Value -= spend.Value;
        broadcaster.Broadcast(spend);
    }
}
