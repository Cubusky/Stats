using Microsoft.CodeAnalysis;

namespace Cubusky.Stats.Generators;

internal readonly record struct StatToGenerate(string ClassName, string TypeName, string? Namespace, StatAttribute StatAttribute)
{
    private static readonly SymbolDisplayFormat GenericTypeFormat = new
    (
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
    );

    public string FullTypeName { get; } = Namespace is string @namespace
        ? $"{Namespace}.{TypeName}"
        : TypeName;

    public string NamespaceDirective { get; } = Namespace is string @namespace
        ? $"namespace {@namespace};"
        : string.Empty;

    public StatToGenerate(INamedTypeSymbol StatSymbol, StatAttribute StatAttribute)
        : this
        (
            ClassName: StatSymbol.Name,
            TypeName: StatSymbol.ToDisplayString(GenericTypeFormat),
            Namespace: StatSymbol.ContainingNamespace?.ToString(),
            StatAttribute: StatAttribute
        )
    { }
}
