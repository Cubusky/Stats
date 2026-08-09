using Microsoft.CodeAnalysis;

namespace Cubusky.Stats.Generators;

internal class StatAttribute
{
    public const string TypeName = nameof(StatAttribute);

    public static readonly string Namespace = typeof(StatAttribute).Namespace;
    public static readonly string FullTypeName = $"{Namespace}.{TypeName}";

    public static bool IsStatAttribute(AttributeData attributeData)
        => attributeData.AttributeClass?.ToString() == FullTypeName;

    public static StatAttribute Single(IEnumerable<AttributeData> attributes)
        => new(attributes.Single(IsStatAttribute));

    private static ArgumentNullException ConstructorArgumentNullException(string argumentName)
        => new(argumentName, $"The {TypeName} attribute must have a {argumentName} specified.");

    public string SyncSubjectPropertyName { get; }
    public string BroadcasterName { get; }
    public string BindingName { get; }

    public StatAttribute(AttributeData statAttribute)
    {
        var arguments = statAttribute.ConstructorArguments;
        SyncSubjectPropertyName = arguments[0].Value as string ?? throw ConstructorArgumentNullException(nameof(SyncSubjectPropertyName));
        BroadcasterName = arguments[1].Value as string ?? throw ConstructorArgumentNullException(nameof(BroadcasterName));
        BindingName = arguments[2].Value as string ?? throw ConstructorArgumentNullException(nameof(BindingName));
    }
}
