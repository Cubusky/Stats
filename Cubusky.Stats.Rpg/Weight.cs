using System.Numerics;

namespace Cubusky.Stats.Rpg;

public partial interface IWeight<TNumber>
    where TNumber : INumberBase<TNumber>;

[Stat]
public partial class Weight<TNumber>(TNumber value, TNumber min, TNumber max, Comparer<TNumber>? comparer = null) : Stat<TNumber>(value, min, max, comparer)
    where TNumber : INumberBase<TNumber>
{
    public Weight(TNumber value, TNumber max, Comparer<TNumber>? comparer = null)
        : this(value, TNumber.Zero, max, comparer) { }

    public Weight(TNumber value, Comparer<TNumber>? comparer = null)
        : this(value, value, comparer) { }

    public static implicit operator Weight<TNumber>(TNumber value) => new(value);
}
