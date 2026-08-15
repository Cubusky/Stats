using Microsoft.CodeAnalysis;

namespace Cubusky.Stats.Generators;

internal static class StatAttribute
{
    public const string TypeName = nameof(StatAttribute);

    public static readonly string Namespace = typeof(StatAttribute).Namespace;
    public static readonly string FullTypeName = $"{Namespace}.{TypeName}";

    public static bool IsStatAttribute(AttributeData attributeData)
        => attributeData.AttributeClass?.ToString() == FullTypeName;
}
