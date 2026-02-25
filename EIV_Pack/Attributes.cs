namespace EIV_Pack;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class EIV_PackableAttribute : Attribute;

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