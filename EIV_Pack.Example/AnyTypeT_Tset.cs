using EIV_Pack.Formatters;

namespace EIV_Pack.Example;

[EIV_Packable]
internal partial class AnyTypeT_Tset<AnyStruct> where AnyStruct : class, new()
{
    public AnyStruct? Value;
}

[EIV_Packable]
internal partial class AnyTypeT_Tset2<AnyStruct> where AnyStruct : IPackable<AnyStruct>, new()
{
    public AnyStruct? Value;
}