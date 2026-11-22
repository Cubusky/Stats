using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cubusky.Stats.Generators.Tests;

public class CachingTest
{
    private static CancellationToken CT => TestContext.Current.CancellationToken;

    public static TheoryData<(string, string, string)> StatToGenerateFactories() => new() { { ("Something.Stupid", "Currency<TNumber>", "Stat<TValue>") }, { ("Something.Stupid.Like", "ILoveYou<TInt>", "Currency<TNumber>") } };

    // Test: are generator inputs equivalent?
    // Justification: generator inputs need to be equivalent so that generators do not run when input is the same.
    [Theory]
    [MemberData(nameof(StatToGenerateFactories))]
    internal void StatToGenerate_SameInput_IsEquivalent((string Namespace, string ClassName, string BaseClassName) factory)
    {
        var statToGenerate = new StatToGenerate(factory.Namespace, factory.ClassName, factory.BaseClassName);
        var other = new StatToGenerate(factory.Namespace, factory.ClassName, factory.BaseClassName);
        statToGenerate.ShouldBeEquivalentTo(other);
    }

    // Test: are generator results cached upon second compilation?
    // Justification: keeps compiler performance optimized
    [Fact]
    public void StatGenerator_SameInput_DoesNotRegenerate()
    {
        const string source = """
            namespace Example;

            [Cubusky.Stats.Stat]
            public partial class Health : Cubusky.Stats.Stat<int>
            {
                public Health() : base(0, 0, 1) { }
            }
            """;

        var generator = new StatGenerator().AsSourceGenerator();
        var compilation = CSharpCompilation.FromSourceCode(source);
        var parseOptions = compilation.SyntaxTrees.OfType<CSharpSyntaxTree>().Select(static tree => tree.Options).FirstOrDefault() ?? CSharpParseOptions.Default;
        var driverOptions = new GeneratorDriverOptions(disabledOutputs: IncrementalGeneratorOutputKind.None, trackIncrementalGeneratorSteps: true);

        var driver = CSharpGeneratorDriver.Create([generator], parseOptions: parseOptions, driverOptions: driverOptions).RunGenerators(compilation, CT);
        var firstResult = driver.GetRunResult().Results.ShouldHaveSingleItem();
        firstResult.Diagnostics.ShouldBeEmpty();
        var firstSource = firstResult.GeneratedSources.ShouldHaveSingleItem();
        firstSource.HintName.ShouldBe("Health.g.cs");
        firstResult.TrackedSteps[StatGenerator.TrackingName.StatsToGenerate].ShouldHaveSingleItem().ShouldHaveSingleInputWithReason(IncrementalStepRunReason.New).ShouldHaveSingleOutputWithReason(IncrementalStepRunReason.New);
        firstResult.TrackedOutputSteps.Values.SelectMany(static steps => steps).ShouldHaveSingleItem().ShouldHaveSingleOutputWithReason(IncrementalStepRunReason.New);

        driver = driver.RunGenerators(compilation.Clone(), CT);
        var secondResult = driver.GetRunResult().Results.ShouldHaveSingleItem();
        secondResult.Diagnostics.ShouldBeEmpty();
        var secondSource = secondResult.GeneratedSources.ShouldHaveSingleItem();
        secondSource.HintName.ShouldBe(firstSource.HintName);
        secondSource.SourceText.ToString().ShouldBe(firstSource.SourceText.ToString());
        secondResult.TrackedSteps[StatGenerator.TrackingName.StatsToGenerate].ShouldHaveSingleItem().ShouldHaveSingleInputWithReason(IncrementalStepRunReason.Modified).ShouldHaveSingleOutputWithReason(IncrementalStepRunReason.Unchanged);
        secondResult.TrackedOutputSteps.Values.SelectMany(static steps => steps).ShouldHaveSingleItem().ShouldHaveSingleInputWithReason(IncrementalStepRunReason.Unchanged).ShouldHaveSingleOutputWithReason(IncrementalStepRunReason.Cached);
    }
}
