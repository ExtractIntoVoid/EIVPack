using Microsoft.CodeAnalysis;

namespace EIV_Pack.Generator;

internal class HelperMethods
{
    public static string ValueName(INamedTypeSymbol symbol)
    {
        return (symbol.IsRecord, symbol.IsValueType) switch
        {
            (true, true) => "record struct",
            (true, false) => "record",
            (false, true) => "struct",
            (false, false) => "class",
        };
    }

    public static string GetTypeParameters(INamedTypeSymbol symbol)
    {
        string typeParameterStr = string.Empty;
        if (symbol.TypeParameters.Length > 0)
        {
            typeParameterStr += "<";
            foreach (var typeParameter in symbol.TypeParameters)
            {
                typeParameterStr += typeParameter.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat);
            }
            typeParameterStr += ">";
        }
        return typeParameterStr;
    }

    public static string GetNameSpace(INamedTypeSymbol symbol)
    {
        List<string> names = [];
        INamespaceSymbol namespaceSymbol = symbol.ContainingNamespace;
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

        return namespaceStr;
    }

    public static IEnumerable<string> GetWhereClauses(INamedTypeSymbol namedType)
    {
        foreach (var tp in namedType.TypeParameters)
        {
            var parts = new List<string>();

            if (tp.HasReferenceTypeConstraint)
                parts.Add("class");

            if (tp.HasUnmanagedTypeConstraint)
                parts.Add("unmanaged");

            if (tp.HasValueTypeConstraint)
                parts.Add("struct");

            if (tp.HasNotNullConstraint)
                parts.Add("notnull");

            if (!tp.ConstraintTypes.IsDefaultOrEmpty)
                parts.AddRange(tp.ConstraintTypes.Select(t => t.ToDisplayString(SymbolDisplayFormat.MinimallyQualifiedFormat)));

            if (tp.HasConstructorConstraint)
                parts.Add("new()");

            if (parts.Count > 0)
            {
                yield return $"where {tp.Name} : {string.Join(", ", parts)}";
            }
        }
    }
}
