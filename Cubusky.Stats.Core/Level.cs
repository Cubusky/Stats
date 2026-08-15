using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using Cubusky.Stats.Generators;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface ILevel<TInteger> : IStat<TInteger>,
    IOperator<Level<TInteger>>,
    IAutoObject<Level<TInteger>.Binding>
    where TInteger : IBinaryInteger<TInteger>
{
    new Level<TInteger>.Binding Bind();
}

[Stat]
public partial class Level<TInteger>
(
    TInteger value,
    TInteger max,
    Comparer<TInteger>? comparer = null
)
    : Stat<TInteger>(value, TInteger.Zero, max, comparer),
    ILevel<TInteger>
    where TInteger : IBinaryInteger<TInteger>
{
    public new Binding Bind() => new(Subject);
}
