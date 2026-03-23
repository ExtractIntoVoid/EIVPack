namespace EIV_Pack;

public enum GenerateType
{
    None,
    VersionTolerant,
    NoGenerate,
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class EIV_PackableAttribute : Attribute
{
    public GenerateType GenerateType { get; } = GenerateType.None;

    public EIV_PackableAttribute() { }

    public EIV_PackableAttribute(GenerateType type)
    {
        GenerateType = type;
    }
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class EIV_PackIgnoreAttribute : Attribute;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class EIV_PackIgnoreFieldsAttribute(params string[] fields) : Attribute
{
    public string[] Fields { get; } = fields;
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class EIV_PackIgnorePropertiesAttribute(params string[] properties) : Attribute
{
    public string[] Properties { get; } = properties;
}

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
public sealed class EIV_PackOrderAttribute(int order) : Attribute
{
    public int Order { get; } = order;
}