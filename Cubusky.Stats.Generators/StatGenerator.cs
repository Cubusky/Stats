using Chickensoft.Collections;
using Chickensoft.Sync;
using Cubusky.BuildingBlocks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Diagnostics.CodeAnalysis;

namespace Cubusky.Stats.Generators;

[Generator]
[SuppressMessage
(
    "MicrosoftCodeAnalysisCorrectness",
    "RS1041:Compiler extensions should be implemented in assemblies targeting netstandard2.0",
    Justification = """
        We want to reference the names of interfaces from Cubusky.BuildingBlocks, which is restricted to netstandard2.1.

        Furthermore, .NET Standard 2.1 can be used under the following conditions:
        1. The generator will work during builds with .NET tooling (e.g. dotnet build or dotnet msbuild), but will not work for builds with .NET Framework tooling (e.g. msbuild)
        2. The generator will not work for IDE scenarios, which includes builds within Visual Studio and all IntelliSense functionality
        """
)]
public class StatGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add the marker attribute
        context.RegisterPostInitializationOutput(ctx =>
        {
            ctx.AddEmbeddedAttributeDefinition();
            ctx.AddSource($"{StatAttribute.TypeName}.g.cs", $$"""
                namespace {{StatAttribute.Namespace}}
                {
                    [global::Microsoft.CodeAnalysis.EmbeddedAttribute]
                    [global::System.AttributeUsage(global::System.AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
                    internal class {{StatAttribute.TypeName}} : global::System.Attribute
                    {
                      public string {{nameof(StatAttribute.SyncSubjectPropertyName)}} { get; }
                      public string {{nameof(StatAttribute.BroadcasterName)}} { get; }
                      public string {{nameof(StatAttribute.BindingName)}} { get; }

                      public {{StatAttribute.TypeName}}(string {{nameof(StatAttribute.SyncSubjectPropertyName).Uncapitalize()}}, string {{nameof(StatAttribute.BroadcasterName).Uncapitalize()}} = "{{nameof(StatAttribute.BroadcasterName)[..^4]}}", string {{nameof(StatAttribute.BindingName).Uncapitalize()}} = "{{nameof(StatAttribute.BindingName)[..^4]}}") 
                      {
                        {{nameof(StatAttribute.SyncSubjectPropertyName)}} = {{nameof(StatAttribute.SyncSubjectPropertyName).Uncapitalize()}};
                        {{nameof(StatAttribute.BroadcasterName)}} = {{nameof(StatAttribute.BroadcasterName).Uncapitalize()}};
                        {{nameof(StatAttribute.BindingName)}} = {{nameof(StatAttribute.BindingName).Uncapitalize()}};
                      }
                    }
                }
                """);
        });

        IncrementalValuesProvider<StatToGenerate> statsToGenerate = context.SyntaxProvider.ForAttributeWithMetadataName
        (
            StatAttribute.FullTypeName,
            predicate: static (node, _) => node is ClassDeclarationSyntax,
            transform: static (ctx, _) =>
                new StatToGenerate
                (
                    (ctx.TargetSymbol as INamedTypeSymbol)!,
                    StatAttribute.Single(ctx.Attributes)
                )
        );

        context.RegisterSourceOutput(statsToGenerate, Execute);
    }

    private static void Execute(SourceProductionContext context, StatToGenerate statToGenerate)
    {
        var statAttribute = statToGenerate.StatAttribute;

        var operationTypeParameter = 'T' + nameof(IOperation<>)[1..];
        var broadcastTypeParameter = 'T' + nameof(IBroadcast<>)[1..];
        var operationParameter = nameof(IOperation<>)[1..].Uncapitalize();
        var broadcastParameter = nameof(IBroadcast<>)[1..].Uncapitalize();

        var operationInterface = $"{nameof(IOperation<>)}<{statToGenerate.TypeName}>";
        var performOpType = $"{nameof(PerformOp<>)}<{statToGenerate.TypeName}>";
        var operationCallbackType = $"{nameof(OperationCallback<,>)}<{statToGenerate.TypeName}, {operationTypeParameter}>";
        var performInterface = $"{nameof(IPerform<>)}<{performOpType}>";

        var operatorInterface = $"{nameof(IOperator<>)}<{statToGenerate.TypeName}>";
        var boxlessQueueType = $"{nameof(BoxlessQueue<>)}<{nameof(IOperation<>)}<{statToGenerate.TypeName}>>";

        var broadcasterInterface = $"{nameof(IBroadcaster<>)}<{statToGenerate.TypeName}>";
        var bindingInterface = $"{nameof(IBinding<,>)}<{statToGenerate.TypeName}, {statToGenerate.TypeName}.{statAttribute.BindingName}>";

        var callbackParameter = nameof(Callback<>).Uncapitalize();
        var conditionParameter = nameof(Condition<>).Uncapitalize();

        var source = $$"""
            #nullable enable
            using {{nameof(Chickensoft)}}.{{nameof(Chickensoft.Collections)}};
            using {{nameof(Chickensoft)}}.{{nameof(Chickensoft.Sync)}};
            using {{nameof(Cubusky)}}.{{nameof(BuildingBlocks)}};

            {{statToGenerate.NamespaceDirective}}
            
            public partial class {{statToGenerate.TypeName}} : {{performInterface}}, {{operatorInterface}}
            {
                private static readonly {{nameof(Blackboard)}} g_DefaultOperationCallbacks = new {{nameof(Blackboard)}}();

                public static void {{nameof(Blackboard.Set)}}<{{operationTypeParameter}}>({{operationCallbackType}} {{operationParameter}})
                    where {{operationTypeParameter}} : struct, {{operationInterface}}
                {
                    g_DefaultOperationCallbacks.{{nameof(Blackboard.Set)}}({{operationParameter}});
                }

                public static void {{nameof(Blackboard.Overwrite)}}<{{operationTypeParameter}}>({{operationCallbackType}} {{operationParameter}})
                    where {{operationTypeParameter}} : struct, {{operationInterface}}
                {
                    g_DefaultOperationCallbacks.{{nameof(Blackboard.Overwrite)}}({{operationParameter}});
                }

                public static new bool {{nameof(Blackboard.Has)}}<{{operationTypeParameter}}>()
                    where {{operationTypeParameter}} : struct, {{operationInterface}}
                {
                    return g_DefaultOperationCallbacks.{{nameof(Blackboard.Has)}}<{{operationCallbackType}}>();
                }

                public static new {{operationCallbackType}} {{nameof(Blackboard.Get)}}<{{operationTypeParameter}}>()
                    where {{operationTypeParameter}} : struct, {{operationInterface}}
                {
                    return g_DefaultOperationCallbacks.{{nameof(Blackboard.Get)}}<{{operationCallbackType}}>();
                }

                private {{statAttribute.BroadcasterName}}? g_OperationBroadcaster;
                private {{statAttribute.BroadcasterName}} G_OperationBroadcaster => g_OperationBroadcaster ??= new {{statAttribute.BroadcasterName}}({{statAttribute.SyncSubjectPropertyName}});
                private readonly {{boxlessQueueType}} g_OperationQueue = new {{boxlessQueueType}}();

                void {{operatorInterface}}.{{nameof(IOperator<>.Perform)}}<{{operationTypeParameter}}>(in {{operationTypeParameter}} {{operationParameter}})
                {
                    g_OperationQueue.Enqueue({{operationParameter}});
                    {{statAttribute.SyncSubjectPropertyName}}.{{nameof(ISyncSubject.Perform)}}(new {{performOpType}}(this, g_DefaultOperationCallbacks, G_OperationBroadcaster));
                }

                void {{performInterface}}.{{nameof(IPerform<>.Perform)}}(in {{performOpType}} op)
                {
                    g_OperationQueue.Dequeue(op);
                }

                public partial class {{statAttribute.BroadcasterName}} : {{broadcasterInterface}}
                {
                    private partial {{nameof(SyncSubject)}}? Subject { get; }

                    void {{broadcasterInterface}}.{{nameof(IBroadcaster<>.Broadcast)}}<{{broadcastTypeParameter}}>(in {{broadcastTypeParameter}} {{broadcastParameter}})
                    {
                        Subject!.{{nameof(SyncSubject.Broadcast)}}({{broadcastParameter}});
                    }
                }

                public partial class {{statAttribute.BindingName}} : {{bindingInterface}}
                {
                    {{statAttribute.BindingName}} {{bindingInterface}}.{{nameof(IBinding<,>.On)}}<{{broadcastTypeParameter}}>({{nameof(Callback<>)}}<{{broadcastTypeParameter}}> {{callbackParameter}}, {{nameof(Condition<>)}}<{{broadcastTypeParameter}}>? {{conditionParameter}})
                    {
                        AddCallback({{callbackParameter}}, {{conditionParameter}});
                        return this;
                    }
                }
            }
            """;

        context.AddSource($"{statToGenerate.ClassName}.g.cs", source);
    }
}

internal static class StringExtensions
{
    public static string Uncapitalize(this string str) =>
        !string.IsNullOrEmpty(str) && char.IsUpper(str[0])
        ? char.ToLowerInvariant(str[0]) + str[1..]
        : str;
}
