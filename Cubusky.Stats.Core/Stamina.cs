using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using Cubusky.Stats.Generators;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface IStamina<TNumber> : IStat<TNumber>,
    IOperator<Stamina<TNumber>>,
    IAutoObject<Stamina<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new Stamina<TNumber>.Binding Bind();
}

[Stat]
public partial class Stamina<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    IStamina<TNumber>
    where TNumber : INumberBase<TNumber>
{
    public new Binding Bind() => new(Subject);
}
