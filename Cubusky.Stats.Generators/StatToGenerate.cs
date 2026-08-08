namespace Cubusky.Stats.Generators;

public readonly record struct StatToGenerate(string ClassName, string TypeName, string? Namespace)
{
    public string FullTypeName { get; } = Namespace is string @namespace
        ? $"{Namespace}.{TypeName}"
        : TypeName;

    public string NamespaceDirective { get; } = Namespace is string @namespace
        ? $"namespace {@namespace};"
        : string.Empty;
}
