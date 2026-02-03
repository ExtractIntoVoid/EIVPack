using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace EIV_Pack.Generator;

[Generator(LanguageNames.CSharp)]
public class MainGenerator : IIncrementalGenerator
{
    public const string AttributeFullName = "EIV_Pack.EIV_PackableAttribute";
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typeDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
               AttributeFullName,
               predicate: static (node, token) =>
               {
                   // search [EIV_Packable] class or struct or record
                   return node is ClassDeclarationSyntax
                                or StructDeclarationSyntax
                                or RecordDeclarationSyntax;
               },
               transform: static (context, token) =>
               {
                   return (TypeDeclarationSyntax)context.TargetNode;
               })
               .WithTrackingName("EIV_Pack.EIV_Packable.1_ForAttributeEIV_PackableAttribute");

        context.RegisterSourceOutput(typeDeclarations.Combine(context.CompilationProvider), static (context, source) =>
        {
            PackGenerator.Generate(source.Left, source.Right, context);
        });
    }
}
