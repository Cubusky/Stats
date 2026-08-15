using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.HitPoints;

public static class Extensions
{
    extension<T>(HitPoints<T> hitPoints)
        where T : INumberBase<T>
    {
        public bool IsDead => T.IsZero(hitPoints.Value) || T.IsNegative(hitPoints.Value);
    }
}
