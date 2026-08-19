using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cubusky.Stats.Generators.Tests;

public sealed class StatGeneratorTest
{
    private static CancellationToken CT => TestContext.Current.CancellationToken;

    private StatGenerator Generator { get; }
    private CSharpCompilation Compilation { get; }
    private GeneratorDriver Driver { get; }
    private GeneratorDriverRunResult Result { get; }

    public StatGeneratorTest()
    {
        Generator = new StatGenerator();
        Compilation = CSharpCompilation.FromSourceCode(SourceText.StatCode);
        Driver = CSharpGeneratorDriver.Create(Generator).RunGenerators(Compilation, CT);
        Result = Driver.GetRunResult();
    }

    [Fact]
    public void Generator_RunDry_EmptyDiagnostics()
    {
        Result.Diagnostics.ShouldBeEmpty();
    }
}
