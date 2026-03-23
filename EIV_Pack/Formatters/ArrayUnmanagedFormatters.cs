namespace EIV_Pack.Formatters;

public sealed class ArrayUnmanagedFormatter<T> : BaseFormatter<T?[]> where T : unmanaged
{
    public override void Serialize(ref PackWriter writer, scoped ref readonly T?[]? value)
    {
        writer.WriteArrayUnmanaged(value);
    }

    public override void Deserialize(ref PackReader reader, scoped ref T?[]? value)
    {
        reader.ReadArray(ref value);
    }
}

public sealed class ArraySegmentUnmanagedFormatter<T> : BaseFormatter<ArraySegment<T?>> where T : unmanaged
{
    public override void Serialize(ref PackWriter writer, scoped ref readonly ArraySegment<T?> value)
    {
        writer.WriteSpanUnmanaged(value.AsSpan());
    }

    public override void Deserialize(ref PackReader reader, scoped ref ArraySegment<T?> value)
    {
        T?[] array = reader.ReadArrayUnmanaged<T>()!;
#if !NETSTANDARD2_0
        value = (ArraySegment<T?>)array;
#else
        value = new ArraySegment<T?>(array);
#endif
    }
}

public sealed class MemoryUnmanagedFormatter<T> : BaseFormatter<Memory<T?>> where T : unmanaged
{
    public override void Serialize(ref PackWriter writer, scoped ref readonly Memory<T?> value)
    {
        writer.WriteSpanUnmanaged(value.Span);
    }

    public override void Deserialize(ref PackReader reader, scoped ref Memory<T?> value)
    {
        value = reader.ReadArrayUnmanaged<T>();
    }
}

public sealed class ReadOnlyMemoryUnmanagedFormatter<T> : BaseFormatter<ReadOnlyMemory<T?>> where T : unmanaged
{
    public override void Serialize(ref PackWriter writer, scoped ref readonly ReadOnlyMemory<T?> value)
    {
        writer.WriteSpanUnmanaged(value.Span);
    }

    public override void Deserialize(ref PackReader reader, scoped ref ReadOnlyMemory<T?> value)
    {
        value = reader.ReadArrayUnmanaged<T>();
    }
}