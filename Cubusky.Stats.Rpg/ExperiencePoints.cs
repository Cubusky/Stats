using System.Numerics;

namespace Cubusky.Stats.Rpg;

public partial interface IExperiencePoints<TNumber>
    where TNumber : INumberBase<TNumber>;

[Stat]
public partial class ExperiencePoints<TNumber>(TNumber value, TNumber min, TNumber max, Comparer<TNumber>? comparer = null) : Stat<TNumber>(value, min, max, comparer)
    where TNumber : INumberBase<TNumber>
{
    public ExperiencePoints(TNumber value, TNumber max, Comparer<TNumber>? comparer = null)
        : this(value, TNumber.Zero, max, comparer) { }

    public ExperiencePoints(TNumber value, Comparer<TNumber>? comparer = null)
        : this(value, value, comparer) { }

    public static implicit operator ExperiencePoints<TNumber>(TNumber value) => new(value);
}
