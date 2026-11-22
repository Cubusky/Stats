using System.Numerics;

namespace Cubusky.Stats.Rpg;

public partial interface IDurability<TNumber>
    where TNumber : INumberBase<TNumber>;

[Stat]
public partial class Durability<TNumber>(TNumber value, TNumber min, TNumber max, Comparer<TNumber>? comparer = null) : Stat<TNumber>(value, min, max, comparer)
    where TNumber : INumberBase<TNumber>
{
    public Durability(TNumber value, TNumber max, Comparer<TNumber>? comparer = null)
        : this(value, TNumber.Zero, max, comparer) { }

    public Durability(TNumber value, Comparer<TNumber>? comparer = null)
        : this(value, value, comparer) { }

    public static implicit operator Durability<TNumber>(TNumber value) => new(value);
}
