using System.Collections.Immutable;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace AdventOfCode.Generation;

[Generator]
public class DayGenerator : IIncrementalGenerator
{
    private const string Attribute = @"
namespace AdventOfCode;

[System.AttributeUsage(System.AttributeTargets.Class)]
public class DayAttribute : System.Attribute
{
}
";
    
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(ctx => ctx.AddSource(
            "DayAttribute.g.cs", 
            SourceText.From(Attribute, Encoding.UTF8)));
        
        IncrementalValuesProvider<ClassDeclarationSyntax> classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;
        
        IncrementalValueProvider<(Compilation Compilation, ImmutableArray<ClassDeclarationSyntax> Classes)> compilationAndClasses
            = context.CompilationProvider.Combine(classDeclarations.Collect());

        context.RegisterSourceOutput(compilationAndClasses,
            static (sourceProductionContext, source) => Execute(source.Compilation, source.Classes, sourceProductionContext));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node) =>
        node is ClassDeclarationSyntax { AttributeLists.Count: > 0 };
    
    private static ClassDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var classDeclarationSyntax = (ClassDeclarationSyntax)context.Node;

        foreach (var attributeListSyntax in classDeclarationSyntax.AttributeLists)
        {
            foreach (var attributeSyntax in attributeListSyntax.Attributes)
            {
                if (context.SemanticModel.GetSymbolInfo(attributeSyntax).Symbol is not IMethodSymbol attributeSymbol)
                {
                    continue;
                }

                var attributeContainingTypeSymbol = attributeSymbol.ContainingType;
                var fullName = attributeContainingTypeSymbol.ToDisplayString();

                if (fullName == "AdventOfCode.DayAttribute")
                {
                    return classDeclarationSyntax;
                }
            }
        }

        return null;
    }
    
    private static void Execute(Compilation compilation, ImmutableArray<ClassDeclarationSyntax> classes, SourceProductionContext context)
    {
        try
        {
            if (classes.IsDefaultOrEmpty)
            {
                return;
            }

            var distinctClasses = classes.Distinct();
            var metaData = distinctClasses.Select(c => DayBuilder.Build(compilation, c)).Where(x => x != null);
            var source = metaData.Select(DayBuilder.Generate!);

            var result = string.Join("\n", source);
            context.AddSource("DayExtensions.g.cs", SourceText.From(result, Encoding.UTF8));
        }        
        catch (Exception ex)
        {
            context.AddSource("DayExtensions.error.cs", SourceText.From(ex.Message + "\n" + ex.StackTrace, Encoding.UTF8));
        }
    }
}