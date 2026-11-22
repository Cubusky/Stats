using System.Numerics;
using Chickensoft.Sync;

namespace Cubusky.Stats.Rpg;

public partial interface IManaPoints<TNumber>
    where TNumber : INumberBase<TNumber>;

[Stat]
public partial class ManaPoints<TNumber>(TNumber value, TNumber min, TNumber max, Comparer<TNumber>? comparer = null) : Stat<TNumber>(value, min, max, comparer), IPerform<ManaPoints<TNumber>.SyncIsDepletedOp>
    where TNumber : INumberBase<TNumber>
{
    public ManaPoints(TNumber value, TNumber max, Comparer<TNumber>? comparer = null)
        : this(value, TNumber.Zero, max, comparer) { }

    public ManaPoints(TNumber value, Comparer<TNumber>? comparer = null)
        : this(value, value, comparer) { }

    public static implicit operator ManaPoints<TNumber>(TNumber value) => new(value);

    public bool IsDepleted => TNumber.IsZero(_value!) || TNumber.IsNegative(_value!);

    protected readonly record struct IsDepletedBroadcast(TNumber Value);

    private readonly record struct SyncIsDepletedOp(Action<TNumber> Callback, Func<TNumber, bool> Condition);

    protected override void Perform(in SetValueOp op)
    {
        base.Perform(op);

        if (IsDepleted)
        {
            Subject.Broadcast(new IsDepletedBroadcast(_value!));
        }
    }

    void IPerform<SyncIsDepletedOp>.Perform(in SyncIsDepletedOp op)
    {
        if (op.Condition(_value!))
        {
            op.Callback(_value!);
        }
    }

    public partial class Binding
    {
        public Binding OnIsDepleted(Action<TNumber> callback, Func<TNumber, bool>? condition = null)
        {
            AddCallback(
                delegate(in IsDepletedBroadcast broadcast)
                {
                    callback(broadcast.Value);
                },
                delegate(in IsDepletedBroadcast broadcast)
                {
                    return predicate(broadcast.Value);
                }
            );

            _subject!.Perform(new SyncIsDepletedOp(callback, predicate));
            return this;

            bool predicate(TNumber value) => condition?.Invoke(value) ?? true;
        }
    }
}
