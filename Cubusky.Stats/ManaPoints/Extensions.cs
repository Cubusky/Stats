using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.ManaPoints;

public static class Extensions
{
    extension<T>(ManaPoints<T> manaPoints)
        where T : INumberBase<T>
    {
        public bool IsDepleted => T.IsZero(manaPoints.Value) || T.IsNegative(manaPoints.Value);
    }
}
