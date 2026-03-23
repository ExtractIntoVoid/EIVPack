using EIV_Pack.Formatters;

namespace EIV_Pack.Test;


internal class VersionTolerant0 : BaseFormatter<VersionTolerant0>, IPackable<VersionTolerant0>
{
    public int Prop1;
    public static void RegisterFormatter()
    {
        if (!FormatterProvider.IsRegistered<VersionTolerant0>())
        {
            FormatterProvider.Register(new VersionTolerant0());
        }
    }

    public static void DeserializePackable(ref PackReader reader, scoped ref VersionTolerant0? value)
    {
        if (!reader.TryReadHeader(out var len) || len == -1)
        {
            value = null;
            return;
        }

        value ??= new();

        for (int i = 0; i < len; i++)
        {
            int order = reader.ReadHeader();
            switch (order)
            {
                default:
                    break;

                case 1:
                    value.Prop1 = reader.ReadUnmanaged<int>();
                    break;
            }
        }
    }

    public static void SerializePackable(ref PackWriter writer, scoped ref readonly VersionTolerant0? value)
    {
        if (value == null)
        {
            writer.WriteHeader();
            return;
        }
        writer.WriteHeader(1);
        writer.WriteUnmanaged<int>(1);
        writer.WriteUnmanaged<int>(value.Prop1);
    }

    public override void Deserialize(ref PackReader reader, scoped ref VersionTolerant0? value)
    {
        DeserializePackable(ref reader, ref value);
    }

    public override void Serialize(ref PackWriter writer, scoped ref readonly VersionTolerant0? value)
    {
        SerializePackable(ref writer, in value);
    }
}

internal class VersionTolerant1 : BaseFormatter<VersionTolerant1>, IPackable<VersionTolerant1>
{
    public int Prop1;
    public int Prop2;
    public static void RegisterFormatter()
    {
        if (!FormatterProvider.IsRegistered<VersionTolerant1>())
        {
            FormatterProvider.Register(new VersionTolerant1());
        }
    }

    public static void DeserializePackable(ref PackReader reader, scoped ref VersionTolerant1? value)
    {
        if (!reader.TryReadHeader(out var len) || len == -1)
        {
            value = null;
            return;
        }

        value ??= new();

        for (int i = 0; i < len; i++)
        {
            int order = reader.ReadHeader();
            switch (order)
            {
                default:
                    break;

                case 1:
                    value.Prop1 = reader.ReadUnmanaged<int>();
                    break;

                case 2:
                    value.Prop2 = reader.ReadUnmanaged<int>();
                    break;
            }
        }
    }

    public static void SerializePackable(ref PackWriter writer, scoped ref readonly VersionTolerant1? value)
    {
        if (value == null)
        {
            writer.WriteHeader();
            return;
        }
        writer.WriteHeader(2);
        writer.WriteUnmanaged<int>(1);
        writer.WriteUnmanaged<int>(value.Prop1);
        writer.WriteUnmanaged<int>(2);
        writer.WriteUnmanaged<int>(value.Prop2);
    }

    public override void Deserialize(ref PackReader reader, scoped ref VersionTolerant1? value)
    {
        DeserializePackable(ref reader, ref value);
    }

    public override void Serialize(ref PackWriter writer, scoped ref readonly VersionTolerant1? value)
    {
        SerializePackable(ref writer, in value);
    }
}

internal class VersionTolerant2 : BaseFormatter<VersionTolerant2>, IPackable<VersionTolerant2>
{
    public int Prop1;
    public int Prop3;
    public static void RegisterFormatter()
    {
        if (!FormatterProvider.IsRegistered<VersionTolerant2>())
        {
            FormatterProvider.Register(new VersionTolerant2());
        }
    }

    public static void DeserializePackable(ref PackReader reader, scoped ref VersionTolerant2? value)
    {
        if (!reader.TryReadHeader(out var len) || len == -1)
        {
            value = null;
            return;
        }

        value ??= new();

        for (int i = 0; i < len; i++)
        {
            int order = reader.ReadHeader();
            switch (order)
            {
                default:
                    break;

                case 1:
                    value.Prop1 = reader.ReadUnmanaged<int>();
                    break;

                case 3:
                    value.Prop3 = reader.ReadUnmanaged<int>();
                    break;
            }
        }
    }

    public static void SerializePackable(ref PackWriter writer, scoped ref readonly VersionTolerant2? value)
    {
        if (value == null)
        {
            writer.WriteHeader();
            return;
        }
        writer.WriteHeader(2);
        writer.WriteUnmanaged<int>(1);
        writer.WriteUnmanaged<int>(value.Prop1);
        writer.WriteUnmanaged<int>(3);
        writer.WriteUnmanaged<int>(value.Prop3);
    }

    public override void Deserialize(ref PackReader reader, scoped ref VersionTolerant2? value)
    {
        DeserializePackable(ref reader, ref value);
    }

    public override void Serialize(ref PackWriter writer, scoped ref readonly VersionTolerant2? value)
    {
        SerializePackable(ref writer, in value);
    }
}

public class VersionTolerantTest
{
    [Fact]
    public void TolerantTest()
    {
        VersionTolerant0.RegisterFormatter();
        VersionTolerant1.RegisterFormatter();
        VersionTolerant2.RegisterFormatter();

        VersionTolerant0 v0 = new() { Prop1 = 44 };
        var data = Serializer.Serialize(v0);
        VersionTolerant0? v0_des = Serializer.Deserialize<VersionTolerant0>(data);
        Assert.NotNull(v0_des);
        Assert.Equal(v0.Prop1, v0_des.Prop1);

        VersionTolerant2 versionTolerant2 = new() { Prop1 = 55 , Prop3 = 5 };
        data = Serializer.Serialize(versionTolerant2);
        VersionTolerant1? v1_des = Serializer.Deserialize<VersionTolerant1>(data);
        Assert.NotNull(v1_des);
        Assert.Equal(versionTolerant2.Prop1, v1_des.Prop1);
        Assert.Equal(0, v1_des.Prop2);
        Assert.Equal(5, versionTolerant2.Prop3);
    }
}
