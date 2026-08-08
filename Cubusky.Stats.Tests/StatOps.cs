//namespace Cubusky.Stats.Tests;

//public class Stat<TValue>(TValue value) : Stat
//{
//    public TValue Value { get; set; } = value;

//    protected override void Perform<TOp>(in TOp op) => Perform(this, op);
//}

//public class Level(int value) : Stat<int>(value)
//{
//    protected override void Perform<TOp>(in TOp op) => Perform(this, op);
//}

//public readonly record struct IncreaseStat<TValue>(TValue Value)
//    : IPerform<Stat<TValue>, IncreaseStat<TValue>>
//{
//    IncreaseStat<TValue> IPerform<Stat<TValue>, IncreaseStat<TValue>>.Perform(Stat<TValue> stat)
//    {
//        stat.Value = Value;
//        return this;
//    }
//}

//public readonly record struct DecreaseLevelMin(int Value)
//    : IPerform<Level, DecreaseLevelMin>
//{
//    DecreaseLevelMin IPerform<Level, DecreaseLevelMin>.Perform(Level stat)
//    {
//        stat.Value = Value;
//        return this;
//    }
//}

//public class Lol
//{
//    public void Method()
//    {
//        var increaseStat = new IncreaseStat<int>(5);
//        var decreaseLevelMin = new DecreaseLevelMin(2);

//        var stat = new Stat<int>(10);
//        stat.Perform(increaseStat);         // Compiles
//        stat.Perform(decreaseLevelMin);     // Compiler Warning (Good!)

//        var level = new Level(10);
//        level.Perform(increaseStat);        // Compiles
//        level.Perform(decreaseLevelMin);    // Compiles

//        using var statBinding = stat.Bind()
//            .On((in IncreaseStat<int> x) => Console.WriteLine(x.Value))
//            .On((in DecreaseLevelMin x) => Console.WriteLine(x.Value));

//        using var levelBinding = level.Bind()
//            .On((in IncreaseStat<int> x) => Console.WriteLine(x.Value))
//            .On((in DecreaseLevelMin x) => Console.WriteLine(x.Value));
//    }
//}