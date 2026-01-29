using System.Buffers;

namespace EIVPack;

public ref partial struct PackWriter : IDisposable
{
    public readonly ReadOnlySequence<byte> GetReadOnlySequence()
    {
        return recyclable.GetReadOnlySequence();
    }

    public readonly ArraySegment<byte> GetAsSegment()
    {
        recyclable.TryGetBuffer(out ArraySegment<byte> buffer);
        return buffer;
    }

    public readonly byte[] GetBytes()
    {
        return recyclable.GetBuffer()[..(int)recyclable.Length];
    }
}
