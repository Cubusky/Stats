using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using Cubusky.Stats.Generators;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface IWeight<TNumber> : IStat<TNumber>,
    IOperator<Weight<TNumber>>,
    IAutoObject<Weight<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new Weight<TNumber>.Binding Bind();
}

[Stat]
public partial class Weight<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    IWeight<TNumber>
    where TNumber : INumberBase<TNumber>
{
    public new Binding Bind() => new(Subject);
}
