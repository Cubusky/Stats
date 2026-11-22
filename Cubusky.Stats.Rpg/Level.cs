using System.Numerics;

namespace Cubusky.Stats.Rpg;

public partial interface ILevel<TInteger>
    where TInteger : IBinaryInteger<TInteger>;

[Stat]
public partial class Level<TInteger>(TInteger value, TInteger min, TInteger max, Comparer<TInteger>? comparer = null) : Stat<TInteger>(value, min, max, comparer)
    where TInteger : IBinaryInteger<TInteger>
{
    public Level(TInteger value, TInteger max, Comparer<TInteger>? comparer = null)
        : this(value, TInteger.Zero, max, comparer) { }

    public Level(TInteger value, Comparer<TInteger>? comparer = null)
        : this(value, value, comparer) { }

    public static implicit operator Level<TInteger>(TInteger value) => new(value);
}
