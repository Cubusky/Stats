using Cubusky.Stats.Generators;
using System.Numerics;

namespace Cubusky.Stats.Core;

[Stat]
public partial class Currency<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer)
    where TNumber : INumberBase<TNumber>
{
}
