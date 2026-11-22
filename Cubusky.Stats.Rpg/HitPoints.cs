using System.Numerics;
using Chickensoft.Sync;

namespace Cubusky.Stats.Rpg;

public partial interface IHitPoints<TNumber>
    where TNumber : INumberBase<TNumber>;

[Stat]
public partial class HitPoints<TNumber>(TNumber value, TNumber min, TNumber max, Comparer<TNumber>? comparer = null) : Stat<TNumber>(value, min, max, comparer), IPerform<HitPoints<TNumber>.SyncIsDeadOp>
    where TNumber : INumberBase<TNumber>
{
    public HitPoints(TNumber value, TNumber max, Comparer<TNumber>? comparer = null)
        : this(value, TNumber.Zero, max, comparer) { }

    public HitPoints(TNumber value, Comparer<TNumber>? comparer = null)
        : this(value, value, comparer) { }

    public static implicit operator HitPoints<TNumber>(TNumber value) => new(value);

    public bool IsDead => TNumber.IsZero(_value!) || TNumber.IsNegative(_value!);

    protected readonly record struct IsDeadBroadcast(TNumber Value);

    private readonly record struct SyncIsDeadOp(Action<TNumber> Callback, Func<TNumber, bool> Condition);

    protected override void Perform(in SetValueOp op)
    {
        base.Perform(op);

        if (IsDead)
        {
            Subject.Broadcast(new IsDeadBroadcast(_value!));
        }
    }

    void IPerform<SyncIsDeadOp>.Perform(in SyncIsDeadOp op)
    {
        if (op.Condition(_value!))
        {
            op.Callback(_value!);
        }
    }

    public partial class Binding
    {
        public Binding OnIsDead(Action<TNumber> callback, Func<TNumber, bool>? condition = null)
        {
            AddCallback(
                delegate(in IsDeadBroadcast broadcast)
                {
                    callback(broadcast.Value);
                },
                delegate(in IsDeadBroadcast broadcast)
                {
                    return predicate(broadcast.Value);
                }
            );

            _subject!.Perform(new SyncIsDeadOp(callback, predicate));
            return this;

            bool predicate(TNumber value) => condition?.Invoke(value) ?? true;
        }
    }
}
