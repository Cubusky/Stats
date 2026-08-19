using Microsoft.CodeAnalysis;
using System.Collections.Immutable;

namespace Cubusky.Stats.Generators;

public readonly record struct StatToGenerate
(
    string ClassName,
    string TypeName,
    string? Namespace,
    ImmutableArray<ContainingTypeInfo> ContainingTypes,
    string? BaseTypeName
) : IEquatable<StatToGenerate>
{
    internal static readonly SymbolDisplayFormat GenericTypeFormat = new
    (
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameOnly,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters
    );

    /// <summary>
    /// Whether any ancestor of this type is itself annotated with [Stat] (not necessarily the immediate
    /// base type - a [Stat] ancestor may be separated by ordinary, non-[Stat] classes). Derived types
    /// inherit the Subject property and just need their Broadcaster/Binding nested types to chain to
    /// the nearest [Stat] ancestor's Broadcaster/Binding.
    /// </summary>
    public bool IsDerived => BaseTypeName is not null;

    public string FullTypeName { get; } = Namespace is string @namespace
        ? $"{Namespace}.{string.Join(".", ContainingTypes.Select(containingType => containingType.Declaration).Append(TypeName))}"
        : string.Join(".", ContainingTypes.Select(containingType => containingType.Declaration).Append(TypeName));

    public StatToGenerate(INamedTypeSymbol StatSymbol)
        : this
        (
            ClassName: StatSymbol.Name,
            TypeName: StatSymbol.ToDisplayString(GenericTypeFormat),
            Namespace: StatSymbol.ContainingNamespace is { IsGlobalNamespace: false } @namespace ? @namespace.ToString() : null,
            ContainingTypes: [.. GetContainingTypes(StatSymbol).Reverse()],
            BaseTypeName: FindNearestStatAncestor(StatSymbol.BaseType)
        )
    { }

    /// <inheritdoc/>
    public readonly bool Equals(StatToGenerate other) =>
        ClassName == other.ClassName &&
        TypeName == other.TypeName &&
        Namespace == other.Namespace &&
        BaseTypeName == other.BaseTypeName &&
        ContainingTypes.SequenceEqual(other.ContainingTypes);

    /// <inheritdoc/>
    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(ClassName);
        hash.Add(TypeName);
        hash.Add(Namespace);
        hash.Add(BaseTypeName);
        foreach (var containingType in ContainingTypes)
        {
            hash.Add(containingType);
        }
        return hash.ToHashCode();
    }

    private static string? FindNearestStatAncestor(INamedTypeSymbol? baseType)
    {
        // Walk the entire base type chain (not just the immediate base type), since a [Stat] ancestor
        // may be separated from this type by one or more ordinary, non-[Stat] classes.
        for (var current = baseType; current is not null; current = current.BaseType)
        {
            if (current.GetAttributes().Any(StatAttribute.IsStatAttribute))
            {
                return current.ToDisplayString(GenericTypeFormat);
            }
        }

        return null;
    }

    private static IEnumerable<ContainingTypeInfo> GetContainingTypes(INamedTypeSymbol StatSymbol)
    {
        for (var containingType = StatSymbol.ContainingType; containingType is not null; containingType = containingType.ContainingType)
        {
            yield return ContainingTypeInfo.Create(containingType);
        }
    }
}

