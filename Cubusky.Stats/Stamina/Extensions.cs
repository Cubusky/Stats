using Cubusky.Stats.Core;
using System.Numerics;

namespace Cubusky.Stats.Stamina;

public static class Extensions
{
    extension<T>(Stamina<T> stamina)
        where T : INumberBase<T>
    {
        public bool IsExhausted => T.IsZero(stamina.Value) || T.IsNegative(stamina.Value);
    }
}
