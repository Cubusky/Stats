using System.Numerics;

namespace Cubusky.Stats.Rpg;

public partial interface ICurrency<TNumber>
    where TNumber : INumberBase<TNumber>;

[Stat]
public partial class Currency<TNumber>(TNumber value, TNumber min, TNumber max, Comparer<TNumber>? comparer = null) : Stat<TNumber>(value, min, max, comparer)
    where TNumber : INumberBase<TNumber>
{
    public Currency(TNumber value, TNumber max, Comparer<TNumber>? comparer = null)
        : this(value, TNumber.Zero, max, comparer) { }

    public Currency(TNumber value, Comparer<TNumber>? comparer = null)
        : this(value, value, comparer) { }

    public static implicit operator Currency<TNumber>(TNumber value) => new(value);
}
