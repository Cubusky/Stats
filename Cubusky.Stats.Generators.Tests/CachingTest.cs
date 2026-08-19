using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit.Sdk;

namespace Cubusky.Stats.Generators.Tests;

public class CachingTest
{
    private static CancellationToken CT => TestContext.Current.CancellationToken;

    public sealed class StatToGenerateFactory(Func<StatToGenerate> Factory) : IXunitSerializable
    {
        public StatToGenerateFactory() : this(null!) { }

        void IXunitSerializable.Serialize(IXunitSerializationInfo info)
            => info.AddValue(nameof(Factory), Factory);

        void IXunitSerializable.Deserialize(IXunitSerializationInfo info)
            => Factory = info.GetValue<Func<StatToGenerate>>(nameof(Factory))!;

        public StatToGenerate Create() => Factory();
    }

    public static TheoryData<StatToGenerateFactory> StatToGenerateFactories()
        => new()
        {
            { new StatToGenerateFactory(() => new StatToGenerate("Stat", "Stat<TValue>", "Something.Stupid", [], null)) },
            { new StatToGenerateFactory(() => new StatToGenerate("Currency", "Currency<TNumber>", "Something.Stupid", [], "Stat<TValue>")) },
        };

    [Fact]
    public void ContainingTypeInfo_SameInput_IsEquivalent()
    {
        var containingTypeInfo = new ContainingTypeInfo("class", "Something.Stupid.Stat<TValue>");
        var other = new ContainingTypeInfo("class", "Something.Stupid.Stat<TValue>");
        containingTypeInfo.ShouldBeEquivalentTo(other);
    }

    [Theory]
    [MemberData(nameof(StatToGenerateFactories))]
    internal void StatToGenerate_SameInput_IsEquivalent(StatToGenerateFactory factory)
    {
        var statToGenerate = factory.Create();
        var other = factory.Create();
        statToGenerate.ShouldBeEquivalentTo(other);
    }

    [Fact]
    public void StatGenerator_SameInput_DoesNotRegenerate()
    {
        var generator = new StatGenerator().AsSourceGenerator();
        var compilation = CSharpCompilation.FromSourceCode(SourceText.StatCode);
        var parseOptions = compilation.SyntaxTrees
            .OfType<CSharpSyntaxTree>()
            .Select(static tree => tree.Options)
            .FirstOrDefault() ?? CSharpParseOptions.Default;
        var driverOptions = new GeneratorDriverOptions
        (
            disabledOutputs: IncrementalGeneratorOutputKind.None,
            trackIncrementalGeneratorSteps: true
        );

        // First run of the generator
        var driver = CSharpGeneratorDriver
            .Create([generator], parseOptions: parseOptions, driverOptions: driverOptions)
            .RunGenerators(compilation, CT);
        driver
            .GetRunResult()
            .Results
            .ShouldHaveSingleItem()
            .TrackedSteps[StatGenerator.TrackingName.StatsToGenerate]
            .ShouldHaveSingleItem()
            .ShouldHaveSingleInputWithReason(IncrementalStepRunReason.New)
            .ShouldHaveSingleOutputWithReason(IncrementalStepRunReason.New);

        // Second run of the generator with the same input
        driver = driver.RunGenerators(compilation.Clone(), CT);
        driver
            .GetRunResult()
            .Results
            .ShouldHaveSingleItem()
            .TrackedSteps[StatGenerator.TrackingName.StatsToGenerate]
            .ShouldHaveSingleItem()
            .ShouldHaveSingleInputWithReason(IncrementalStepRunReason.Modified)
            .ShouldHaveSingleOutputWithReason(IncrementalStepRunReason.Unchanged);
    }
}
