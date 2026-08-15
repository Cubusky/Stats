using Microsoft.CodeAnalysis;

namespace Cubusky.Stats.Generators;

/// <summary>Describes a single containing type (a class/struct/interface/record that the target [Stat] type is nested inside of), so the generator can re-declare a matching partial wrapper around the generated members instead of emitting an unrelated top-level type.</summary>
internal readonly record struct ContainingTypeInfo(string Keyword, string Declaration)
{
    public static ContainingTypeInfo Create(INamedTypeSymbol type) => new
    (
        Keyword: type switch
        {
            { TypeKind: TypeKind.Interface } => "interface",
            { TypeKind: TypeKind.Struct, IsRecord: true } => "record struct",
            { TypeKind: TypeKind.Struct } => "struct",
            { IsRecord: true } => "record",
            _ => "class"
        },
        Declaration: type.ToDisplayString(StatToGenerate.GenericTypeFormat)
    );
}
