//using Chickensoft.Collections;
//using Chickensoft.Sync;
//using Chickensoft.Sync.Primitives;

//namespace Cubusky.Stats;

////public interface IExperiencePointsBroadcast;

////public interface IExperiencePointsBroadcaster : IStatBroadcaster
////{
////    void ExperiencePointsBroadcast<TBroadcast>(in TBroadcast broadcast)
////        where TBroadcast : struct, IBroadcast<ExperiencePoints>;
////}

////public interface IBroadcast<in TConformance>;

////public interface IExperiencePointsBinding
////{
////    IExperiencePointsBinding On<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
////        where TBroadcast : struct, IExperiencePointsBroadcast;
////}

////public interface IOp<in TConformance>;

//public class ExperiencePoints(int max) : Stat<int>(0, 0, max),
//    //IExperiencePointsBroadcaster,
//    IPerform<ExperiencePoints.PerformOp>,
//    IAutoObject<ExperiencePoints.Binding>
//{
//    private readonly record struct PerformOp(ExperiencePoints Stat, IReadOnlyBlackboard ExperiencePointsOpCallbacks) : IBoxlessValueHandler<IOp<ExperiencePoints>>
//    {
//        void IBoxlessValueHandler<IOp<ExperiencePoints>>.HandleValue<TValue1>(in TValue1 value)
//        {
//            var experiencePointsOpCallback = ExperiencePointsOpCallbacks.Get<Action<ExperiencePoints, TValue1>>();
//            experiencePointsOpCallback(Stat, value);
//        }
//    }

//    private readonly Blackboard _experiencePointsOpCallbacks = new();
//    private readonly BoxlessQueue<IOp<ExperiencePoints>> _experiencePointsOpQueue = new();

//    public void ExperiencePointsBroadcast<TBroadcast>(in TBroadcast broadcast)
//        where TBroadcast : struct, IBroadcast<ExperiencePoints>
//    {
//        _subject.Broadcast(in broadcast);
//    }

//    //public void Perform(Action<IExperiencePointsBroadcaster> callback)
//    //{
//    //    _subject.Perform(new PerformOp(callback));
//    //}

//    public void SetExperiencePointsOp<TExperiencePointsOp>(Action<ExperiencePoints, TExperiencePointsOp> callback)
//        where TExperiencePointsOp : struct, IOp<ExperiencePoints>
//    {
//        _experiencePointsOpCallbacks.Set(callback);
//    }

//    public void PerformExperiencePointsOp<TExperiencePointsOp>(in TExperiencePointsOp statOp)
//        where TExperiencePointsOp : struct, IOp<ExperiencePoints>
//    {
//        _experiencePointsOpQueue.Enqueue(statOp);
//        _subject.Perform(new PerformOp(this, _experiencePointsOpCallbacks));
//    }

//    void IPerform<PerformOp>.Perform(in PerformOp op)
//    {
//        _experiencePointsOpQueue.Dequeue(op);
//    }

//    public new Binding Bind() => new(_subject);

//    public new class Binding : Stat<int>.Binding
//    {
//        internal Binding(ISyncSubject subject) : base(subject) { }

//        //IExperiencePointsBinding IExperiencePointsBinding.On<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition)
//        //{
//        //    AddCallback(callback, condition);
//        //    return this;
//        //}

//        public Binding OnExperiencePoints<TBroadcast>(Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
//            where TBroadcast : struct, IBroadcast<ExperiencePoints>
//        {
//            AddCallback(callback, condition);
//            return this;
//        }
//    }
//}

//public interface ICoinsBroadcast;

//public interface ICoinsBroadcaster
//{
//    void Broadcast<TBroadcast>(in TBroadcast broadcast)
//        where TBroadcast : struct, ICoinsBroadcast;
//}

//public class Coins(int max) : Stat<int>(0, 0, max),
//    ICoinsBroadcaster,
//    IPerform<Coins.PerformOp>
//{
//    private readonly record struct PerformOp(Action<ICoinsBroadcaster> Callback);

//    public void Perform(Action<ICoinsBroadcaster> callback)
//    {
//        _subject.Perform(new PerformOp(callback));
//    }

//    void ICoinsBroadcaster.Broadcast<TBroadcast>(in TBroadcast broadcast)
//    {
//        _subject.Broadcast(in broadcast);
//    }

//    void IPerform<PerformOp>.Perform(in PerformOp op)
//    {
//        op.Callback(this);
//    }
//}

//public readonly record struct IncreaseOp(int Value) : IOp<Stat<int>>;
//public readonly record struct Increase(int Value) : IBroadcast<Stat<int>>;
//public readonly record struct AddOp(int Value) : IOp<ExperiencePoints>;
//public readonly record struct Add(int Value) : IBroadcast<ExperiencePoints>;
//public readonly record struct Double : ICoinsBroadcast;

//public class Lol
//{
//    public void Method()
//    {
//        var stat = new Stat<int>(5, 1, 10);
//        var exp = new ExperiencePoints(100);
//        var coins = new Coins(100);

//        stat.Set<int, IncreaseOp>(static (_, _) => { });
//        stat.Set(static (Stat<int> stat, IncreaseOp increase) =>
//        {
//            stat.Value += increase.Value;
//            stat.Broadcast(new Increase(increase.Value));
//        });

//        stat.Set<int, AddOp>(static (_, _) => { });
//        stat.Set<int, AddOp>(static (stat, add) =>
//        {
//            stat.Value += add.Value;
//            stat.Broadcast(new Add(add.Value));
//        });

//        exp.Set<int, IncreaseOp>(static (stat, increase) => { });
//        exp.Set(static (Stat<int> stat, IncreaseOp increase) =>
//        {
//            stat.Value += increase.Value;
//            stat.Broadcast(new Increase(increase.Value));
//            stat.Broadcast(new Add(increase.Value));
//        });

//        exp.Set<AddOp>(static (_, _) => { });
//        exp.Set<AddOp>(static (stat, add) =>
//        {
//            stat.Value += add.Value;
//            stat.Broadcast(new Increase(add.Value));
//            stat.Broadcast(new Add(add.Value));
//        });

//        stat.Perform(new IncreaseOp(3));
//        stat.Perform(new AddOp(3));

//        exp.Perform(new IncreaseOp(3));
//        exp.Perform(new AddOp(3));

//        using var statBinding = stat.Bind()
//            .On(static (in Increase increase) => { });

//        using var expBinding = exp.Bind()
//            .On(static (in Add add) => { })
//            .On(static (in Increase increase) => { });
//    }
//}

//public static class LolExtensions
//{
//    public static void Broadcast<TValue, TBroadcast>(this Stat<TValue> stat, in TBroadcast broadcast)
//        where TBroadcast : struct, IBroadcast<Stat<TValue>>
//    {
//        stat.StatBroadcast(in broadcast);
//    }

//    public static void Broadcast<TBroadcast>(this ExperiencePoints exp, in TBroadcast broadcast)
//        where TBroadcast : struct, IBroadcast<ExperiencePoints>
//    {
//        exp.ExperiencePointsBroadcast(in broadcast);
//    }

//    public static Stat<TValue>.Binding On<TValue, TBroadcast>(this Stat<TValue>.Binding binding, Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
//        where TBroadcast : struct, IBroadcast<Stat<TValue>>
//    {
//        binding.OnStat(callback, condition);
//        return binding;
//    }

//    public static ExperiencePoints.Binding On<TBroadcast>(this ExperiencePoints.Binding binding, Callback<TBroadcast> callback, Condition<TBroadcast>? condition = null)
//        where TBroadcast : struct, IBroadcast<ExperiencePoints>
//    {
//        binding.OnExperiencePoints(callback, condition);
//        return binding;
//    }

//    public static void Perform<TValue, TStatOp>(this Stat<TValue> stat, in TStatOp op)
//        where TStatOp : struct, IOp<Stat<TValue>>
//    {
//        stat.PerformStatOp(in op);
//    }

//    public static void Perform<TExperiencePointsOp>(this ExperiencePoints exp, in TExperiencePointsOp op)
//        where TExperiencePointsOp : struct, IOp<ExperiencePoints>
//    {
//        exp.PerformExperiencePointsOp(in op);
//    }

//    public static void Set<TValue, TStatOp>(this Stat<TValue> stat, Action<Stat<TValue>, TStatOp> callback)
//        where TStatOp : struct, IOp<Stat<TValue>>
//    {
//        stat.SetStatOp(callback);
//    }

//    public static void Set<TExperiencePointsOp>(this ExperiencePoints exp, Action<ExperiencePoints, TExperiencePointsOp> callback)
//        where TExperiencePointsOp : struct, IOp<ExperiencePoints>
//    {
//        exp.SetExperiencePointsOp(callback);
//    }
//}