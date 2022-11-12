using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdventOfCode.Generation;

public static class SampleBuilder
{
    public static MethodSamples? Build(Compilation compilation, MethodDeclarationSyntax methodDeclarationSyntax)
    {
        var semanticModel = compilation.GetSemanticModel(methodDeclarationSyntax.SyntaxTree);
        if (ModelExtensions.GetDeclaredSymbol(semanticModel, methodDeclarationSyntax) is not IMethodSymbol methodSymbol)
        {
            // something went wrong!
            return null;
        }

        if (!methodSymbol.Name.StartsWith("Part"))
        {
            // wrong name
            return null;
        }

        var ns = methodSymbol.ContainingNamespace.Name;
        var className = methodSymbol.ContainingType.Name;
        var name = methodSymbol.Name.Replace("Part", "Tests");
        var resultType = methodSymbol.ReturnType.ToDisplayString();

        var samples = new List<Sample>();
        foreach (var attribute in methodSymbol.GetAttributes())
        {
            if (attribute.AttributeClass?.ToDisplayString() != "AdventOfCode.SampleAttribute")
            {
                continue;
            }

            if (attribute.ConstructorArguments.Length != 2)
            {
                continue;
            }
            
            var input = attribute.ConstructorArguments[0].ToCSharpString();
            var value = attribute.ConstructorArguments[1].ToCSharpString();
            
            samples.Add(new Sample(input, value));
        }
        
        return new MethodSamples(ns, className, name, resultType, samples);
    }
    
    public static string Generate(MethodSamples methodSamples)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"namespace {methodSamples.Namespace} {{");
        sb.AppendLine($"    public partial class {methodSamples.Class} {{");
        sb.AppendLine($"        protected override IEnumerable<(string, {methodSamples.ResultType})> {methodSamples.SampleName} {{ get; }} = new[]");
        sb.AppendLine("        {");
        foreach (var sample in methodSamples.Samples)
        {
            sb.AppendLine($"            ({sample.Input}, {sample.Value}),");
        }
        sb.AppendLine("        };");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

}

public class MethodSamples
{
    public MethodSamples(string ns, string @class, string sampleName, string resultType, IReadOnlyList<Sample> samples)
    {
        Namespace = ns;
        Class = @class;
        SampleName = sampleName;
        ResultType = resultType;
        Samples = samples;
    }

    public string Namespace { get; }
    public string Class { get; }
    public string SampleName { get; }
    public string ResultType { get; }
    public IReadOnlyList<Sample> Samples { get; }
}
public class Sample
{
    public Sample(string input, string value)
    {
        Input = input;
        Value = value;
    }

    public string Input { get; }
    public string Value { get; }
}