namespace EIV_Pack.Formatters;

public sealed class NullableUnmanagedFormatter<T> : BaseFormatter<T?> where T : unmanaged
{
    public override void Deserialize(ref PackReader reader, scoped ref T? value)
    {
        value = reader.ReadUnmanagedNullable<T>();
    }

    public override void Serialize(ref PackWriter writer, scoped ref readonly T? value)
    {
        writer.WriteUnmanagedNullable(value);
    }
}
