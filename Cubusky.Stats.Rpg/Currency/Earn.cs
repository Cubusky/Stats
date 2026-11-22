using System.Numerics;
using Cubusky.BuildingBlocks;
using Cubusky.BuildingBlocks.Collections;

namespace Cubusky.Stats.Rpg.Currency;

public readonly record struct Earn<TNumber>(TNumber Value) : IOperation<Currency<TNumber>>
    where TNumber : INumberBase<TNumber>
{
    static Earn()
    {
        OperationMap.Shared.Add<Currency<TNumber>, Earn<TNumber>, Currency<TNumber>.Broadcaster>(Callback);
    }

    public static void Callback(Currency<TNumber> currency, in Earn<TNumber> earn, Currency<TNumber>.Broadcaster broadcaster)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(earn.Value, nameof(earn.Value));

        if (currency.TrySet(currency.Value + earn.Value, out var oldValue))
        {
            broadcaster.Broadcast(new Earn<TNumber>(currency.Value - oldValue));
        }
    }
}
