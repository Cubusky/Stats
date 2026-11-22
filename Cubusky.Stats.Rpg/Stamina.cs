using System.Numerics;
using Chickensoft.Sync;

namespace Cubusky.Stats.Rpg;

public partial interface IStamina<TNumber>
    where TNumber : INumberBase<TNumber>;

[Stat]
public partial class Stamina<TNumber>(TNumber value, TNumber min, TNumber max, Comparer<TNumber>? comparer = null) : Stat<TNumber>(value, min, max, comparer), IPerform<Stamina<TNumber>.SyncIsExhaustedOp>
    where TNumber : INumberBase<TNumber>
{
    public Stamina(TNumber value, TNumber max, Comparer<TNumber>? comparer = null)
        : this(value, TNumber.Zero, max, comparer) { }

    public Stamina(TNumber value, Comparer<TNumber>? comparer = null)
        : this(value, value, comparer) { }

    public static implicit operator Stamina<TNumber>(TNumber value) => new(value);

    public bool IsExhausted => TNumber.IsZero(_value!) || TNumber.IsNegative(_value!);

    protected readonly record struct IsExhaustedBroadcast(TNumber Value);

    private readonly record struct SyncIsExhaustedOp(Action<TNumber> Callback, Func<TNumber, bool> Condition);

    protected override void Perform(in SetValueOp op)
    {
        base.Perform(op);

        if (IsExhausted)
        {
            Subject.Broadcast(new IsExhaustedBroadcast(_value!));
        }
    }

    void IPerform<SyncIsExhaustedOp>.Perform(in SyncIsExhaustedOp op)
    {
        if (op.Condition(_value!))
        {
            op.Callback(_value!);
        }
    }

    public partial class Binding
    {
        public Binding OnIsExhausted(Action<TNumber> callback, Func<TNumber, bool>? condition = null)
        {
            AddCallback(
                delegate(in IsExhaustedBroadcast broadcast)
                {
                    callback(broadcast.Value);
                },
                delegate(in IsExhaustedBroadcast broadcast)
                {
                    return predicate(broadcast.Value);
                }
            );

            _subject!.Perform(new SyncIsExhaustedOp(callback, predicate));
            return this;

            bool predicate(TNumber value) => condition?.Invoke(value) ?? true;
        }
    }
}
