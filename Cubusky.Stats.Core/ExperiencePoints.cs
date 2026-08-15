using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using Cubusky.Stats.Generators;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface IExperiencePoints<TNumber> : IStat<TNumber>,
    IOperator<ExperiencePoints<TNumber>>,
    IAutoObject<ExperiencePoints<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new ExperiencePoints<TNumber>.Binding Bind();
}

[Stat]
public partial class ExperiencePoints<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    IExperiencePoints<TNumber>
    where TNumber : INumberBase<TNumber>
{
    public new Binding Bind() => new(Subject);
}
