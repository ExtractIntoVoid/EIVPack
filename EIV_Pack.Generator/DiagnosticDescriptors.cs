using Microsoft.CodeAnalysis;

namespace EIV_Pack.Generator;

internal static class DiagnosticDescriptors
{
    const string Category = "GenerateEIVPack";

    public static readonly DiagnosticDescriptor MustBePartial = new(
        id: "EIVPACK001",
        title: "EIVPackable object must be partial",
        messageFormat: "The EIVPackable object '{0}' must be partial",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor NoFieldOrProperties = new(
        id: "EIVPACK002",
        title: "EIVPackable object must have a property or field",
        messageFormat: "The EIVPackable object '{0}' must have a field or proprety",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor VersionTolerantNoReadOnly = new(
        id: "EIVPACK003",
        title: "EIVPackable object cannot serialize ReadOnly/Init in VersionTolerant",
        messageFormat: "The EIVPackable object '{0}' cannot serialize ReadOnly/Init ('{1}') in VersionTolerant",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);


    public static readonly DiagnosticDescriptor VersionTolerantMustOrder = new(
        id: "EIVPACK004",
        title: "EIVPackable object property or field must have Order attribute in VersionTolerant",
        messageFormat: "The EIVPackable object '{0}' property or field '{1}' must have Order attribute in VersionTolerant",
        category: Category,
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);
}
