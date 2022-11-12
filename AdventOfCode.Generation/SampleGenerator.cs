using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AdventOfCode.Generation;

[Generator]
public class SampleGenerator : IIncrementalGenerator
{
    private const string Attribute = @"
namespace AdventOfCode;

[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
public class SampleAttribute : System.Attribute
{
    public SampleAttribute(string input, object expected) { }
}
";
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "SampleAttribute.g.cs", 
            SourceText.From(Attribute, Encoding.UTF8)));
        
        IncrementalValuesProvider<MethodDeclarationSyntax> methodsWithSamples = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;
        
        IncrementalValueProvider<(Compilation Compilation, ImmutableArray<MethodDeclarationSyntax> Methods)> compilationAndClasses
            = context.CompilationProvider.Combine(methodsWithSamples.Collect());

        context.RegisterSourceOutput(compilationAndClasses,
            static (sourceProductionContext, source) => Execute(source.Compilation, source.Methods, sourceProductionContext));
    }
    
    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
        node is MethodDeclarationSyntax { AttributeLists.Count: > 0 };
    
    private static MethodDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var methodDeclarationSyntax = (MethodDeclarationSyntax)context.Node;

        foreach (var attributeListSyntax in methodDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "AdventOfCode.SampleAttribute")
                {
                    return methodDeclarationSyntax;
                }
            }
        }

        return null;
    }
    
    private static void Execute(Compilation compilation, ImmutableArray<MethodDeclarationSyntax> methods, SourceProductionContext context)
    {
        if (methods.IsDefaultOrEmpty)
        {
            return;
        }

        var distinctMethods = methods.Distinct();
        var samples = distinctMethods.Select(c => SampleBuilder.Build(compilation, c)).Where(x => x != null);
        var source = samples.Select(SampleBuilder.Generate!);
        
        var result = string.Join("\n", source);
        context.AddSource("SampleExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
    }
}