using Microsoft.CodeAnalysis;

namespace Cubusky.Stats.Generators;

internal readonly record struct StatToGenerate(string Namespace, string ClassName, string BaseClassName)
{
    // Analyzers can't load Cubusky.Stats at runtime, so these are plain strings instead of typeof(...) lookups.
    internal const string StatsNamespace = "Cubusky.Stats";
    internal const string StatAttributeFullName = StatsNamespace + "." + nameof(StatAttribute);

    internal static readonly SymbolDisplayFormat GenericTypeFormat = new(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly, genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters);

    public StatToGenerate(INamedTypeSymbol StatType)
        : this(Namespace: StatType.ContainingNamespace.ToDisplayString(), ClassName: StatType.ToDisplayString(GenericTypeFormat), BaseClassName: FindNearestStatAncestor(StatType.BaseType)) { }

    private static string FindNearestStatAncestor(INamedTypeSymbol? baseType)
    {
        // Walk the entire base type chain (not just the immediate base type), since a [Stat] ancestor
        // may be separated from this type by one or more ordinary, non-[Stat] classes.
        for (var current = baseType; current is not null; current = current.BaseType)
        {
            if (IsStatType(current) || current.GetAttributes().Any(IsStatAttribute))
            {
                return current.ToDisplayString(GenericTypeFormat);
            }
        }

        throw new InvalidOperationException($"Cannot generate a stat for a type that does not inherit from {nameof(Stat<>)}<TValue> or has the {nameof(StatAttribute)}");
    }

    private static bool IsStatType(INamedTypeSymbol type) => type.OriginalDefinition is { Name: nameof(Stat<>), Arity: 1 } definition && definition.ContainingNamespace.ToDisplayString() == StatsNamespace;

    private static bool IsStatAttribute(AttributeData attributeData) => attributeData.AttributeClass is { Name: nameof(StatAttribute) } attributeClass && attributeClass.ContainingNamespace.ToDisplayString() == StatsNamespace;
}
