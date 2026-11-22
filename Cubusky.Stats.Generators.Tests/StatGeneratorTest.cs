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
        var source = """
            namespace Example;

            [Cubusky.Stats.Stat]
            public partial class Health : Cubusky.Stats.Stat<int>
            {
                public Health() : base(0, 0, 1) { }
            }
            """;

        Generator = new StatGenerator();
        Compilation = CSharpCompilation.FromSourceCode(source);
        Driver = CSharpGeneratorDriver.Create(Generator).RunGenerators(Compilation, CT);
        Result = Driver.GetRunResult();
    }

    // Test: generator does not return warnings or errors
    // Justification: it should generate clean results.
    [Fact]
    public void Generator_RunDry_EmptyDiagnostics()
    {
        Result.Diagnostics.ShouldBeEmpty();
    }

    // Test: generator produces interface and class
    // Justification: backwards compatibility
    [Fact]
    public void Generator_RunDry_ProducesInterface()
    {
        var source = Result.GetSource("Health.g.cs").SourceText.ToString();
        source.ShouldContain("public partial interface IHealth : IStat<Int32>, IOperator<Health>, IAutoObject<Health.Binding>");
        source.ShouldContain("public partial class Health : IHealth, IOperator<Health>, IAutoObject<Health.Binding>");
    }

    // Test: generator produces binding and broadcaster
    // Justification: backwards compatibility
    [Fact]
    public void Generator_RunDry_ProducesBindingAndBroadcaster()
    {
        var source = Result.GetSource("Health.g.cs").SourceText.ToString();
        source.ShouldContain("public partial class Health");
        source.ShouldContain("public new partial class Binding : Stat<Int32>.Binding");
        source.ShouldContain("public new partial class Broadcaster : Stat<Int32>.Broadcaster");
    }

    // Test: generator produces interface methods
    // Justification: backwards compatibility
    [Fact]
    public void Generator_RunDry_ProducesInterfaceMethods()
    {
        var source = Result.GetSource("Health.g.cs").SourceText.ToString();
        source.ShouldContain("new void Perform<TOperation>(in TOperation operation)");
        source.ShouldContain("new Health.Binding Bind();");
        source.ShouldContain("void IOperator<Health>.Perform<TOperation>(in TOperation operation)");
    }

    // Test: generator produces binding methods
    // Justification: backwards compatibility
    [Fact]
    public void Generator_RunDry_ProducesBindingMethods()
    {
        var source = Result.GetSource("Health.g.cs").SourceText.ToString();
        source.ShouldContain("public new Binding On<TOperation>(Callback<TOperation> callback, Condition<TOperation>? condition = null)");
    }
}
