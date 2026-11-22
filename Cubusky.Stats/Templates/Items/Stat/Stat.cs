using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using Cubusky.Stats.Generators;/* USING_NUMERICS */

namespace MY_NAMESPACE;

public interface IMY_STAT<MY_VALUE> : IStat<MY_VALUE>,
    IOperator<MY_STAT<MY_VALUE>>,
    IAutoObject<MY_STAT<MY_VALUE>.Binding>/* TYPE_CONSTRAINT */
{
    new MY_STAT<MY_VALUE>.Binding Bind();
}

[Stat]
public partial class MY_STAT<MY_VALUE>
(
    MY_VALUE value,/* MIN_PARAMETER */
    MY_VALUE max,
    Comparer<MY_VALUE>? comparer = null
)
    : Stat<MY_VALUE>(value/* MIN_VALUE */, max, comparer),
    IMY_STAT<MY_VALUE>/* TYPE_CONSTRAINT */
{
    public new Binding Bind() => new(Subject);
}
