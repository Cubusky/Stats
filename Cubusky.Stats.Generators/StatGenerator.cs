using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Cubusky.Stats.Generators;

[Generator]
public class StatGenerator : IIncrementalGenerator
{
    private static readonly SymbolDisplayFormat GenericTypeFormat = new
    (
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
    );

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add the marker attribute
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddEmbeddedAttributeDefinition();
            ctx.AddSource("StatAttribute.g.cs", """
                namespace Cubusky.Stats.Generators;

                [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
                internal class StatAttribute : global::System.Attribute;
                """);
        });

        IncrementalValuesProvider<StatToGenerate> statsToGenerate = context.SyntaxProvider.ForAttributeWithMetadataName
        (
            "Cubusky.Stats.Generators.StatAttribute",
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (ctx, _) => GetStatToGenerate((ctx.TargetSymbol as INamedTypeSymbol)!)
        );

        context.RegisterSourceOutput(statsToGenerate, Execute);
    }

    private static StatToGenerate GetStatToGenerate(INamedTypeSymbol statSymbol)
    {
        return new StatToGenerate
        (
            ClassName: statSymbol.Name,
            TypeName: statSymbol.ToDisplayString(GenericTypeFormat),
            Namespace: statSymbol.ContainingNamespace?.ToString()
        );
    }

    private static void Execute(SourceProductionContext context, StatToGenerate statToGenerate)
    {
        var source = $$"""
            {{statToGenerate.NamespaceDirective}}
            
            public partial class {{statToGenerate.TypeName}}
            {
                // This is a generated class. You can add your own members here.
            }
            """;

        context.AddSource($"{statToGenerate.ClassName}.g.cs", source);
    }
}
