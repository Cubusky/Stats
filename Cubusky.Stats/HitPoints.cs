//using System.Numerics;

//namespace Cubusky.Stats;

//public interface IHitPoints<TNumber> : IStat<TNumber>
//    where TNumber : INumberBase<TNumber>;

//public class HitPoints<TNumber>(TNumber value, TNumber max, Comparer<TNumber>? comparer = null)
//    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
//    IHitPoints<TNumber>
//    where TNumber : INumberBase<TNumber>
//{

//}

//public readonly record struct Damage<TNumber>(TNumber Value)
//    : IStatBroadcast<TNumber>
//    where TNumber : INumberBase<TNumber>;

//public readonly record struct Heal<TNumber>(TNumber Value)
//    : IStatBroadcast<TNumber>
//    where TNumber : INumberBase<TNumber>;

////public static class Lol
////{
////    public static void Damage<TNumber>(this HitPoints<TNumber> hitPoints, TNumber amount)
////        where TNumber : INumberBase<TNumber>
////    {
////        ArgumentOutOfRangeException.ThrowIfNegative(amount, nameof(amount));

////        hitPoints.Perform<Damage<TNumber>>(broadcast =>
////        {
////            hitPoints.TrySet(hitPoints.Value - amount, out var oldValue);
////            broadcast(new Damage<TNumber>(oldValue - hitPoints.Value));
////        });

////        //hitPoints.TrySet(hitPoints.Value - amount, out var oldValue);
////        //hitPoints.Broadcast(new HitPoints<TNumber>.DamageBroadcast(oldValue - hitPoints.Value));
////    }
////}

////public interface IHitPoints<TNumber> : IStat<TNumber>
////    where TNumber : INumber<TNumber>
////{
////    void Damage(TNumber amount);
////    void Heal(bool ifDead = false);
////    void Heal(TNumber amount, bool ifDead = false);
////    void Revive(TNumber amount);
////    void Increase(TNumber amount, bool heal = false);
////    void Decrease(TNumber amount);
////}

////public class HitPoints<TNumber>(TNumber value, TNumber max, Comparer<TNumber>? comparer = null)
////    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
////    IHitPoints<TNumber>
////    where TNumber : INumber<TNumber>
////{
////    public readonly record struct DamageBroadcast(TNumber Value) : IStatBroadcast<TNumber>;
////    public readonly record struct HealBroadcast(TNumber Value) : IStatBroadcast<TNumber>;
////    public readonly record struct ReviveBroadcast(TNumber Value) : IStatBroadcast<TNumber>;
////    public readonly record struct IncreaseBroadcast(TNumber Value) : IStatBroadcast<TNumber>;
////    public readonly record struct DecreaseBroadcast(TNumber Value) : IStatBroadcast<TNumber>;

////    public void Damage(TNumber amount)
////    {
////        ArgumentOutOfRangeException.ThrowIfNegative(amount, nameof(amount));

////        this.TrySet(Value - amount, out var oldValue);
////        Broadcast(new DamageBroadcast(oldValue - Value));
////    }

////    public void Heal(bool ifDead = false) => Heal(Max, ifDead);
////    public void Heal(TNumber amount, bool ifDead = false)
////    {
////        ArgumentOutOfRangeException.ThrowIfNegative(amount, nameof(amount));
////        if (Value <= TNumber.Zero && !ifDead)
////        {
////            return;
////        }

////        this.TrySet(Value + amount, out var oldValue);
////        Broadcast(new HealBroadcast(Value - oldValue));
////    }

////    public void Revive(TNumber amount)
////    {
////        ArgumentOutOfRangeException.ThrowIfNegative(amount, nameof(amount));
////        if (Value > TNumber.Zero)
////        {
////            return;
////        }

////        this.TrySet(amount, out _);
////        Broadcast(new ReviveBroadcast(Value));
////    }

////    public void Increase(TNumber amount, bool heal = false)
////    {
////        ArgumentOutOfRangeException.ThrowIfNegative(amount, nameof(amount));

////        Broadcast(new IncreaseBroadcast(Max += amount));
////        if (heal)
////        {
////            Heal(amount);
////        }
////    }

////    public void Decrease(TNumber amount)
////    {
////        ArgumentOutOfRangeException.ThrowIfNegative(amount, nameof(amount));

////        this.TrySetMax(Max - amount, out var oldMax);
////        Broadcast(new DecreaseBroadcast(oldMax - Max));
////    }
////}