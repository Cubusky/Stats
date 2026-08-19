using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Cubusky.Stats.Generators.Tests;

internal static class Extensions
{
    extension(GeneratorDriverRunResult Result)
    {
        public GeneratedSourceResult GetSource(string hintName)
            => Result.GetSources().Single(source => source.HintName == hintName);

        public IEnumerable<GeneratedSourceResult> GetSources()
            => Result.Results.SelectMany(static result => result.GeneratedSources);
    }

    extension(CSharpCompilation)
    {
        public static CSharpCompilation FromSourceCode(params IEnumerable<string> sources)
            => CSharpCompilation.Create
            (
                assemblyName: "compilation",
                syntaxTrees: [.. sources.Select(static source => CSharpSyntaxTree.ParseText(source))],
                references: CSharpCompilation.GetReferences(),
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
            );

        // The generated code references types from the BCL as well as Chickensoft.Sync, Chickensoft.Collections and Cubusky.BuildingBlocks. Rather than hand-picking a handful of assemblies (which is easy to get wrong and silently produces "missing namespace/reference" diagnostics), reference every assembly the test host itself was resolved with. All of these are already sitting next to the test binary because they are (transitive) dependencies of the generator project under test.
        private static MetadataReference[] GetReferences()
            =>
            [..
                ((string)AppContext.GetData("TRUSTED_PLATFORM_ASSEMBLIES")!)
                .Split(Path.PathSeparator)
                .Select(path => MetadataReference.CreateFromFile(path))
            ];
    }

    extension(IncrementalGeneratorRunStep RunStep)
    {
        public IncrementalGeneratorRunStep ShouldHaveSingleInputWithReason(IncrementalStepRunReason expected)
        {
            var (source, outputIndex) = RunStep.Inputs.ShouldHaveSingleItem();
            source.Outputs[outputIndex].Reason.ShouldBe(expected);
            return RunStep;
        }

        public IncrementalGeneratorRunStep ShouldHaveSingleOutputWithReason(IncrementalStepRunReason expected)
        {
            var reason = RunStep.Outputs.ShouldHaveSingleItem().Reason;
            reason.ShouldBe(expected);
            return RunStep;
        }
    }
}
