using Chickensoft.Sync;
using Chickensoft.Sync.Primitives;
using Cubusky.BuildingBlocks;
using Cubusky.Stats.Generators;
using System.Numerics;

namespace Cubusky.Stats.Core;

public interface ICurrency<TNumber> : IStat<TNumber>,
    IOperator<Currency<TNumber>>,
    IAutoObject<Currency<TNumber>.Binding>
    where TNumber : INumberBase<TNumber>
{
    new Currency<TNumber>.Binding Bind();
}

[Stat(nameof(_subject))]
public partial class Currency<TNumber>
(
    TNumber value,
    TNumber max,
    Comparer<TNumber>? comparer = null
)
    : Stat<TNumber>(value, TNumber.Zero, max, comparer),
    ICurrency<TNumber>
    where TNumber : INumberBase<TNumber>
{
    public new partial class Broadcaster : Stat<TNumber>.Broadcaster
    {
        private partial SyncSubject? Subject => _subject;

        internal Broadcaster(SyncSubject subject) : base(subject) { }
    }

    public new Binding Bind() => new(_subject);

    public new partial class Binding : Stat<TNumber>.Binding
    {
        internal Binding(ISyncSubject subject) : base(subject) { }
    }
}
