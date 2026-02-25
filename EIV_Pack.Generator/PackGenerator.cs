using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Text;

namespace EIV_Pack.Generator;

internal static class PackGenerator
{
    internal static void Generate(GeneratorClass generatorClass, SourceProductionContext context)
    {
        TypeDeclarationSyntax syntax = generatorClass.Syntax;
        var semanticModel = generatorClass.Compilation.GetSemanticModel(syntax.SyntaxTree);

        var typeSymbol = semanticModel.GetDeclaredSymbol(syntax, context.CancellationToken);
        if (typeSymbol == null)
        {
            return;
        }

        // verify is partial
        if (!syntax.Modifiers.Any(static m => m.IsKind(SyntaxKind.PartialKeyword)))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MustBePartial, syntax.Identifier.GetLocation(), typeSymbol.Name));
            return;
        }

        // return on private
        if (typeSymbol.DeclaredAccessibility == Accessibility.Private)
            return;

        var fieldOrParamList = GetFieldOrParams(typeSymbol);

        RemoveIgnored(ref fieldOrParamList, typeSymbol);

        if (fieldOrParamList.Count == 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.NoFieldOrProperties, syntax.Identifier.GetLocation(), typeSymbol.Name));
            return;
        }

        var initOnly = fieldOrParamList.Where(x => x is IPropertySymbol property && property.SetMethod != null && property.SetMethod.IsInitOnly).ToList();
        initOnly.AddRange(fieldOrParamList.Where(x => x is IFieldSymbol fieldSymbol && fieldSymbol.IsRequired));
        initOnly.AddRange(fieldOrParamList.Where(x => x is IPropertySymbol property && property.IsRequired));

        var fullType = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
           .Replace("global::", "")
           .Replace("<", "_")
           .Replace(">", "_");

        var classOrStructOrRecord = (typeSymbol.IsRecord, typeSymbol.IsValueType) switch
        {
            (true, true) => "record struct",
            (true, false) => "record",
            (false, true) => "struct",
            (false, false) => "class",
        };

        StringBuilder sb = new();
        sb.AppendLine("// Generated with EIV_Pack.Generator.");
        sb.AppendLine("using EIV_Pack;");
        sb.AppendLine("using EIV_Pack.Formatters;");
        sb.AppendLine();

        if (generatorClass.IsNet8OrGreater)
        {
            sb.AppendLine("#nullable enable");
            sb.AppendLine();
        }

        List<string> names = [];
        INamespaceSymbol namespaceSymbol = typeSymbol.ContainingNamespace;
        while (namespaceSymbol != null)
        {
            if (namespaceSymbol.Name.Contains("<global namespace>"))
                break;

            names.Add(namespaceSymbol.Name);
            namespaceSymbol = namespaceSymbol.ContainingNamespace;
        }

        names.Reverse();

        string namespaceStr = string.Empty;

        if (names.Count != 0)
            namespaceStr = string.Join(".", names);

        if (names.Count > 1)
            namespaceStr = namespaceStr.Substring(1);

        if (!string.IsNullOrEmpty(namespaceStr))
        {
            if (generatorClass.IsNet8OrGreater)
            {
                sb.AppendLine($"namespace {namespaceStr};");
            }
            else
            {
                sb.AppendLine($"namespace {namespaceStr}");
                sb.AppendLine("{");
            }
        }

        sb.AppendLine();
        sb.AppendLine($"partial {classOrStructOrRecord} {typeSymbol.Name} : IPackable<{typeSymbol.Name}>, IFormatter<{typeSymbol.Name}>");
        sb.AppendLine("{");

        if (!typeSymbol.GetMembers().Any(x => x.IsStatic && x.Kind == SymbolKind.Method && x.Name == ".cctor"))
        {
            sb.AppendLine(
                $$"""

                    static {{typeSymbol.Name}}()
                    {
                        RegisterFormatter();
                    }
                
                """);
        }

        bool hasRegister = HasRegisterFormatter(typeSymbol);

        string initForRegister = string.Empty;

        if (initOnly.Count > 0)
        {
            StringBuilder initBuilder = new();
            initBuilder.AppendLine("\n\t\t\t{");
            foreach (var item in initOnly)
            {
                
                initBuilder.AppendLine($"\t\t\t\t{item.Name} = default,");
            }

            initBuilder.Append("\t\t\t}");
            initForRegister = initBuilder.ToString();
        }

        sb.AppendLine(
            $$"""

                public static{{(hasRegister ? " new " : " ")}}void RegisterFormatter()
                {
                    if (!FormatterProvider.IsRegistered<{{typeSymbol.Name}}>())
                    {
                        FormatterProvider.Register<{{typeSymbol.Name}}>(new {{typeSymbol.Name}}(){{initForRegister}});
                    }

                    if (!FormatterProvider.IsRegistered<{{typeSymbol.Name}}[]>())
                    {
                        FormatterProvider.Register(new ArrayFormatter<{{typeSymbol.Name}}>());
                    }
                }
            
            """);

        GeneratePackable(ref syntax, ref typeSymbol, ref sb, ref fieldOrParamList, ref initOnly, generatorClass.IsNet8OrGreater);



        sb.AppendLine(
            $$"""

                public void Deserialize(ref PackReader reader, scoped ref {{typeSymbol.Name}}{{(typeSymbol.IsValueType ? string.Empty : generatorClass.IsNet8OrGreater ? "?" : string.Empty)}} value)
                {
                    DeserializePackable(ref reader, ref value);
                }

                public void Serialize(ref PackWriter writer, scoped ref readonly {{typeSymbol.Name}}{{(typeSymbol.IsValueType ? string.Empty : generatorClass.IsNet8OrGreater ? "?" : string.Empty)}} value)
                {
                    SerializePackable(ref writer, in value);
                }
            
            """);

        sb.AppendLine("}");

        if (!string.IsNullOrEmpty(namespaceStr) && !generatorClass.IsNet8OrGreater)
        {
            sb.AppendLine();
            sb.AppendLine("}");
        }

        context.AddSource($"{fullType}.g.cs", sb.ToString());
    }


    internal static void GeneratePackable(ref TypeDeclarationSyntax _, ref INamedTypeSymbol typeSymbol, ref StringBuilder sb, ref List<ISymbol> FieldAndParamList, ref List<ISymbol> initOnly, bool isNet8OrGreater)
    {
        var nullable = typeSymbol.IsValueType ? string.Empty : isNet8OrGreater ? "?" : string.Empty;
        var exceptInit = FieldAndParamList.Except(initOnly);
        bool hasInitOnly = initOnly.Count > 0;
        string ending = hasInitOnly ? string.Empty : ";";

        sb.AppendLine($"\tconst int EIV_PACK_FieldAndParamCount = {FieldAndParamList.Count};");
        sb.AppendLine();
        sb.AppendLine($"\tpublic static void DeserializePackable(ref PackReader reader, scoped ref {typeSymbol.Name}{nullable} value)");
        sb.AppendLine("\t{");
        if (FieldAndParamList.Count <= 255)
            sb.AppendLine($"\t\tif (!reader.TryReadSmallHeader(out byte header) || header != EIV_PACK_FieldAndParamCount)");
        else
            sb.AppendLine($"\t\tif (!reader.TryReadHeader(out int header) || header != EIV_PACK_FieldAndParamCount)");

        if (!typeSymbol.IsValueType)
            sb.AppendLine(
                $$"""
                            {
                                value = null;
                                return;
                            }

                            value ??= new(){{ending}}
                    """);
        else
            sb.AppendLine($"\t\tvalue = new(){ending}");

        if (hasInitOnly)
        {
            sb.AppendLine("\t\t{");

            foreach (var item in initOnly)
            {
                WriteDesParam(ref sb, item, false);
            }

            sb.AppendLine("\t\t};");
        }

        foreach (var item in exceptInit)
        {
            WriteDesParam(ref sb, item);
        }

        sb.AppendLine("\t}");
        sb.AppendLine();
        sb.AppendLine($"\tpublic static void SerializePackable(ref PackWriter writer, scoped ref readonly {typeSymbol.Name}{nullable} value)");
        sb.AppendLine("\t{");

        string useSmall = FieldAndParamList.Count <= 255 ? "Small" : string.Empty;

        if (!typeSymbol.IsValueType)
        {
            sb.AppendLine(
                $$"""
                    if (value == null)
                    {
                        writer.Write{{useSmall}}Header();
                        return;
                    }
                """);
        }

        sb.AppendLine($"\t\twriter.Write{useSmall}Header(EIV_PACK_FieldAndParamCount);");

        foreach (var item in initOnly)
        {
            WriteSerParam(ref sb, item);
        }

        foreach (var item in exceptInit)
        {
            WriteSerParam(ref sb, item);
        }

        sb.AppendLine("\t}");
    }

    internal static void WriteSerParam(ref StringBuilder sb, ISymbol symbol)
    {
        ITypeSymbol? type = null;
        switch (symbol)
        {
            case IFieldSymbol fieldSymbol:
                type = fieldSymbol.Type;
                break;
            case IPropertySymbol propertySymbol:
                type = propertySymbol.Type;
                break;
            default:
                break;
        }

        if (type == null)
        {
            sb.AppendLine($"// ERROR CANT GENERATE : {symbol.Name}");
            return;
        }

        if (type is INamedTypeSymbol namedType)
        {
            if (type.IsUnmanagedType && !namedType.IsGenericType)
            {
                sb.AppendLine($"\t\twriter.WriteUnmanaged<{type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>(value.{symbol.Name});");
                return;
            }

            if (type.SpecialType == SpecialType.System_String)
            {
                sb.AppendLine($"\t\twriter.WriteString(value.{symbol.Name});");
                return;
            }
        }

        if (type is IArrayTypeSymbol arrayTypeSymbol)
        {
            sb.AppendLine($"\t\twriter.WriteArray<{arrayTypeSymbol.ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>(value.{symbol.Name});");
            return;
        }

        sb.AppendLine($"\t\twriter.WriteValue<{type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>(value.{symbol.Name});");
    }

    internal static void WriteDesParam(ref StringBuilder sb, ISymbol symbol, bool useValue = true)
    {
        string val = useValue ? "value." : "\t";
        string endColon = useValue ? ";" : ",";
        ITypeSymbol? type = null;
        NullableAnnotation nullableAnnotation = NullableAnnotation.None;
        switch (symbol)
        {
            case IFieldSymbol fieldSymbol:
                nullableAnnotation = fieldSymbol.NullableAnnotation;
                type = fieldSymbol.Type;
                break;
            case IPropertySymbol propertySymbol:
                nullableAnnotation = propertySymbol.NullableAnnotation;
                type = propertySymbol.Type;
                break;
            default:
                break;
        }

        if (type == null)
        {
            sb.AppendLine($"// ERROR CANT GENERATE : {symbol.Name}");
            return;
        }

        if (type is INamedTypeSymbol namedType)
        {
            if (type.IsUnmanagedType && !namedType.IsGenericType)
            {
                sb.AppendLine($"\t\t{val}{symbol.Name} = reader.ReadUnmanaged<{type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>(){endColon}");
                return;
            }

            if (type.SpecialType == SpecialType.System_String)
            {
                sb.AppendLine($"\t\t{val}{symbol.Name} = reader.ReadString(){(nullableAnnotation == NullableAnnotation.NotAnnotated ? "!" : string.Empty)}{endColon}");
                return;
            }
        }

        if (type is IArrayTypeSymbol arrayTypeSymbol)
        {
            sb.AppendLine($"\t\t{val}{symbol.Name} = reader.ReadArray<{arrayTypeSymbol.ElementType.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>(){(nullableAnnotation == NullableAnnotation.NotAnnotated ? " ?? []" : string.Empty)}{endColon}");
            return;
        }

        sb.AppendLine($"\t\t{val}{symbol.Name} = reader.ReadValue<{type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)}>(){(nullableAnnotation == NullableAnnotation.NotAnnotated ? "!" : string.Empty)}{endColon}");
    }

    static List<ISymbol> GetFieldOrParams(INamedTypeSymbol typeSymbol)
    {
        List<ISymbol> symbols = [];

        if (typeSymbol.BaseType != null)
        {
            symbols.AddRange(GetFieldOrParams(typeSymbol.BaseType));
        }

        var fields = typeSymbol.GetMembers().OfType<IFieldSymbol>().Where(
            f => !f.IsStatic
            && !f.IsAbstract
            && !f.IsImplicitlyDeclared
            && !f.IsReadOnly
            && f.DeclaredAccessibility != Accessibility.Private
            && f.Name != "EqualityContract"
            && !f.GetAttributes().Any(att => att.AttributeClass?.Name == "EIV_PackIgnoreAttribute")
            ).ToList();

        symbols.AddRange(fields);

        var properties = typeSymbol.GetMembers().OfType<IPropertySymbol>().Where(
            p => !p.IsStatic
            && !p.IsReadOnly
            && p.DeclaredAccessibility != Accessibility.Private
            && p.GetMethod != null && p.GetMethod.DeclaredAccessibility != Accessibility.Private
            && p.SetMethod != null && p.SetMethod.DeclaredAccessibility != Accessibility.Private
            && !p.GetAttributes().Any(att => att.AttributeClass?.Name == "EIV_PackIgnoreAttribute")
            );

        symbols.AddRange(properties);

        return symbols;
    }

    static bool HasRegisterFormatter(INamedTypeSymbol typeSymbol)
    {
        bool ret = false;

        if (typeSymbol.BaseType != null)
        {
            ret |= typeSymbol.BaseType.GetAttributes().Any(att => att.AttributeClass?.Name == "EIV_PackableAttribute");
            if (!ret)
                ret |= HasRegisterFormatter(typeSymbol.BaseType);
        }

        return ret;
    }

    static void RemoveIgnored(ref List<ISymbol> symbols, INamedTypeSymbol typeSymbol)
    {
        List<string> propertiesToRemove = [];
        List<string> fieldsToIgnore = [];
        foreach (var item in typeSymbol.GetAttributes())
        {
            if (item.AttributeClass == null)
                continue;

            if (item.AttributeClass.Name == "EIV_PackIgnorePropertiesAttribute")
            {
                var ctorArg = item.ConstructorArguments[0];
                if (ctorArg.Kind == TypedConstantKind.Array && ctorArg.Values != null)
                {
                    foreach (var element in ctorArg.Values)
                    {
                        if (element.Value is string s)
                            propertiesToRemove.Add(s);
                    }
                }
                else if (ctorArg.Value is string single)
                {
                    propertiesToRemove.Add(single);
                }
            }

            if (item.AttributeClass.Name == "EIV_PackIgnoreFieldsAttribute")
            {
                var ctorArg = item.ConstructorArguments[0];
                if (ctorArg.Kind == TypedConstantKind.Array && ctorArg.Values != null)
                {
                    foreach (var element in ctorArg.Values)
                    {
                        if (element.Value is string s)
                            fieldsToIgnore.Add(s);
                    }
                }
                else if (ctorArg.Value is string single)
                {
                    fieldsToIgnore.Add(single);
                }
            }
        }

        symbols.RemoveAll(s => s.Kind == SymbolKind.Property && propertiesToRemove.Contains(s.Name));
        symbols.RemoveAll(s => s.Kind == SymbolKind.Field && fieldsToIgnore.Contains(s.Name));
    }
}
