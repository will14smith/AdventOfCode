using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdventOfCode.Generation;

public static class DayBuilder
{
    private static readonly Regex DayNumberMatch = new(".*Day(\\d+)", RegexOptions.Compiled);

    public static Day? Build(Compilation compilation, ClassDeclarationSyntax classDeclarationSyntax)
    {
        var semanticModel = compilation.GetSemanticModel(classDeclarationSyntax.SyntaxTree);
        if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is not INamedTypeSymbol classSymbol)
        {
            // something went wrong!
            return null;
        }

        var ns = classSymbol.ContainingNamespace.Name;
        var cls = classSymbol.Name;
        var day = int.Parse(DayNumberMatch.Match(cls).Groups[1].Value);
        
        return new Day(ns, cls, day);
    }
    
    public static string Generate(Day metaData)
    {
        var sb = new StringBuilder();
        
        sb.AppendLine($"namespace {metaData.Namespace} {{");
        sb.AppendLine($"    public partial class {metaData.Class} {{");
        sb.AppendLine($"        public {metaData.Class}(ITestOutputHelper output) : base({metaData.Number}, output) {{ }}");
        sb.AppendLine("    }");
        sb.AppendLine("}");

        return sb.ToString();
    }

}