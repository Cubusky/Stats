//using Chickensoft.Sync.Primitives;
//using System.Numerics;

//namespace Cubusky.Stats;

//public interface IStamina<TNumber> : INumberStat<TNumber>,
//    IAutoObject<Stamina<TNumber>.Binding>
//    where TNumber : INumber<TNumber>
//{
//    void Consume(TNumber amount);
//    void Recover(bool ifExhausted = false);
//    void Recover(TNumber amount, bool ifExhausted = false);
//    void Increase(TNumber amount, bool recover = false);
//    void Decrease(TNumber amount);
//}

//public class Stamina<TNumber>(TNumber value, TNumber max, IEqualityComparer<TNumber>? comparer = null)
//    : NumberStat<TNumber>(value, max, comparer: comparer),
//    IStamina<TNumber>
//    where TNumber : INumber<TNumber>
//{
//    public void Consume(TNumber amount)
//    {
//        throw new NotImplementedException();
//    }

//    public void Recover(bool ifExhausted = false)
//    {
//        throw new NotImplementedException();
//    }

//    public void Recover(TNumber amount, bool ifExhausted = false)
//    {
//        throw new NotImplementedException();
//    }

//    public void Increase(TNumber amount, bool recover = false)
//    {
//        throw new NotImplementedException();
//    }

//    public void Decrease(TNumber amount)
//    {
//        throw new NotImplementedException();
//    }
//}
