namespace EIV_Pack.Example;

[EIV_Packable]
public partial class MyTestClass
{
    public string? strrr;
    public RuntimeFieldHandle rfh;
    public KeyValuePair<int, int> fsdfsf;
    public int yeet;
    public int[]? yeetArray;
    public int Yeet { get; set; }

    public List<int> test = [];
}

[EIV_Packable]
public partial record class MyTestRecordClass
{
    public int yeet;
    public int Yeet { get; set; }
}

[EIV_Packable]
public partial record struct MyTestRecordStruct
{
    public int yeet;
    public int Yeet { get; set; }
}

[EIV_Packable]
public partial struct MyTestStruct
{
    public int yeet;
    public int Yeet { get; set; }
}

[EIV_Packable]
internal partial class MyTestClassInternal
{
    public int yeet;
    public int Yeet { get; set; }
}

[EIV_Packable]
internal partial record class MyTestRecordClassInternal
{
    public int yeet;
    public int Yeet { get; set; }
}

[EIV_Packable]
internal partial record struct MyTestRecordStructInternal
{
    public int yeet;
    public int Yeet { get; set; }
}

[EIV_Packable]
internal partial struct MyTestStructInternal
{
    public int yeet;
    public int Yeet { get; set; }
}


[EIV_Packable]
internal partial struct OrderTest
{
    [EIV_PackOrder(1)]
    public int Order1;
    [EIV_PackOrder(2)]
    public int Order2;
    [EIV_PackOrder(4)]
    public int Order4;
    [EIV_PackOrder(3)]
    public int Order3;

    public int NotInOrder;
    public int NotInOrder3;
    public int NotInOrder2;
}

[EIV_Packable(GenerateType.NoGenerate)]
internal partial class DONOTGENERATE;



[EIV_Packable(GenerateType.VersionTolerant)]
internal partial class Version1
{
    [EIV_PackOrder(1)]
    public int Order1;
    [EIV_PackOrder(2)]
    public int Order2;
}

[EIV_Packable(GenerateType.VersionTolerant)]
internal partial class Version2
{
    [EIV_PackOrder(1)]
    public int Order1;
    [EIV_PackOrder(2)]
    public int Order2;
    [EIV_PackOrder(3)]
    public int Order3;
}

[EIV_Packable(GenerateType.VersionTolerant)]
internal partial class Version4
{
    [EIV_PackOrder(1)]
    public int Order1;
    [EIV_PackOrder(2)]
    public int Order2;
    [EIV_PackOrder(4)]
    public int Order4;
}


[EIV_Packable(GenerateType.VersionTolerant)]
internal partial struct Version4Struct
{
    [EIV_PackOrder(1)]
    public int Order1;
    [EIV_PackOrder(2)]
    public int Order2;
    [EIV_PackOrder(4)]
    public int Order4;
}