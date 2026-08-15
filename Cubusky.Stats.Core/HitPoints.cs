using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using Cubusky.Stats.Generators;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface IHitPoints<TNumber> : IStat<TNumber>,
    IOperator<HitPoints<TNumber>>,
    IAutoObject<HitPoints<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new HitPoints<TNumber>.Binding Bind();
}

[Stat]
public partial class HitPoints<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    IHitPoints<TNumber>
    where TNumber : INumberBase<TNumber>
{
    public new Binding Bind() => new(Subject);
}
