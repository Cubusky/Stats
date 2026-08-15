using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using Cubusky.Stats.Generators;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface IDurability<TNumber> : IStat<TNumber>,
    IOperator<Durability<TNumber>>,
    IAutoObject<Durability<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new Durability<TNumber>.Binding Bind();
}

[Stat]
public partial class Durability<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    IDurability<TNumber>
    where TNumber : INumberBase<TNumber>
{
    public new Binding Bind() => new(Subject);
}
