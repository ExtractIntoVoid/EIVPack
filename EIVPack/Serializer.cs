namespace EIVPack;

public static class Serializer
{
    public static byte[] SerializeArray<T>(in T?[]? value)
    {
        if (value == null)
            return [];

        if (value.Length == 0)
            return Constants.EmptyCollection.ToArray();

        using PackWriter writer = new();
        writer.WriteArray(value);
        byte[] bytes = writer.GetBytes();
        writer.Dispose();
        return bytes;
    }

    public static T?[]? DeserializeArray<T>(in byte[] bytes)
    {
        if (bytes.Length == 0)
            return null;

        if (Constants.EmptyCollection.SequenceEqual(bytes))
            return [];

        PackReader reader = new(bytes);
        return reader.ReadArray<T>();
    }

    public static byte[] Serialize<T>(in T? value)
    {
        using PackWriter writer = new();
        writer.WriteValue(value);
        byte[] bytes = writer.GetBytes();
        writer.Dispose();
        return bytes;
    }

    public static T? Deserialize<T>(in byte[] bytes)
    {
        T? value = default;
        PackReader reader = new(bytes);
        reader.ReadValue(ref value);
        return value;
    }
}
