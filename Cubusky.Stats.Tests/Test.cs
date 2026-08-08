using Cubusky.BuildingBlocks;
using System.Numerics;

namespace Cubusky.Stats.Tests;

public class Test
{
    // 1. Use IBuildingBlock<TConformance> for both operation and broadcast
    private readonly record struct Increase<TValue>(TValue Value) : IBuildingBlock<Stat<TValue>>
        where TValue : INumberBase<TValue>;

    // 2. Use IOperation<TConformance> and IBroadcast<TConformance> to keep operation and broadcast separate
    private readonly record struct DoubleOperation<TValue> : IOperation<ExperiencePoints<TValue>>
        where TValue : INumberBase<TValue>;

    private readonly record struct DoubleBroadcast<TValue>(TValue Value) : IBroadcast<ExperiencePoints<TValue>>
        where TValue : INumberBase<TValue>;

    // 3. Define your callbacks
    private static void IncreaseCallback(in Stat<int> stat, in Increase<int> increase, in IBroadcaster<Stat<int>> broadcaster)
    {
        stat.Value += increase.Value;
        broadcaster.Broadcast(increase);
    }

    private static void DoubleCallback(in ExperiencePoints<int> experiencePoints, in DoubleOperation<int> doubleOp, in IBroadcaster<ExperiencePoints<int>> broadcaster)
    {
        experiencePoints.Value *= 2;
        broadcaster.Broadcast(new DoubleBroadcast<int>(experiencePoints.Value));
        broadcaster.Broadcast(new Increase<int>(experiencePoints.Value));
        //broadcaster.Broadcast(doubleOp); // Does not compile, because DoubleOperation<int> is not a broadcast.
    }

    // 4. Use the callbacks in your code
    public void Method()
    {
        // Stat
        var stat = new Stat<int>(5, 0, 10);
        var sharedStat = stat as IStat<int>;
        using var statBinding = sharedStat.Bind();

        // Compiles
        Stat<int>.Set<Increase<int>>(IncreaseCallback);
        Stat<int>.Set((OperationCallback<Stat<int>, Increase<int>>)IncreaseCallback);
        sharedStat.Perform(new Increase<int>(5));
        statBinding
            .On(static (in Increase<int> increase) => { })
            .On<int, Increase<int>>(OnIncrease);

        //// Does not compile
        //SyncSubject subject;
        //using var newStatBinding = new Stat<int>.Binding(subject);
        //var statBroadcaster = new Stat<int>.Broadcaster(subject);

        //Stat<int>.Set<DoubleOperation<int>>(DoubleCallback);
        //Stat<int>.Set(IncreaseCallback);
        //sharedStat.Perform(new DoubleOperation<int>());
        //statBinding.On(static (in DoubleBroadcast<int> doubleBr) => { });
        //statBinding.On<int, DoubleBroadcast<int>>(OnDouble);
        //statBinding.On(OnIncrease);

        // ExperiencePoints
        var exp = new ExperiencePoints<int>(0, 100);
        var sharedExp = exp as IExperiencePoints<int>;
        using var expBinding = sharedExp.Bind();

        // Compiles
        ExperiencePoints<int>.Set<DoubleOperation<int>>(DoubleCallback);
        sharedExp.Perform(new Increase<int>(10));
        sharedExp.Perform(new DoubleOperation<int>());

        expBinding
            .On(static (in DoubleBroadcast<int> doubleBr) => { })
            .On<int, DoubleBroadcast<int>>(OnDouble)
            .OnValue(static (int i) => { })
            .On(static (in Increase<int> increase) => { })
            .On<int, Increase<int>>(OnIncrease);

        //// Does not compile
        //using var newExpBinding = new ExperiencePoints<int>.Binding(subject);
        //var expBroadcaster = new ExperiencePoints<int>.Broadcaster(subject);

        //// ExperiencePoints broadcasts must come before stat broadcasts.
        //expBinding
        //    .OnValue(static (int i) => { })
        //    .On(static (in DoubleBroadcast<int> doubleBr) => { });

        // Compiles, but is potentially confusing because Increase<int> is not an operation for ExperiencePoints<int>
        ExperiencePoints<int>.Set<Increase<int>>(IncreaseCallback);

        // Local functions
        static void OnIncrease(in Increase<int> increase)
        {
            Console.WriteLine($"Stat increased by {increase.Value}");
        }

        static void OnDouble(in DoubleBroadcast<int> doubleBr)
        {
            Console.WriteLine($"Exp doubled, new value: {doubleBr.Value}");
        }
    }
}
