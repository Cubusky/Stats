using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using Cubusky.Stats.Generators;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface IManaPoints<TNumber> : IStat<TNumber>,
    IOperator<ManaPoints<TNumber>>,
    IAutoObject<ManaPoints<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new ManaPoints<TNumber>.Binding Bind();
}

[Stat]
public partial class ManaPoints<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    IManaPoints<TNumber>
    where TNumber : INumberBase<TNumber>
{
    public new Binding Bind() => new(Subject);
}
