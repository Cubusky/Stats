using Chickensoft.Sync;

namespace Cubusky.Stats.Generators.Tests;

public static class SourceText
{
    public const string StatCode = $$"""
        using {{nameof(Chickensoft)}}.{{nameof(Chickensoft.Sync)}};
        using {{nameof(Cubusky)}}.{{nameof(Stats)}}.{{nameof(Generators)}};

        namespace Something.Stupid;

        [Stat]
        public partial class Stat<TValue>
        {
            protected partial {{nameof(SyncSubject)}} Subject => field;

            public Stat()
            {
                Subject = new(this);
            }
        }
        """;
}
