using System.Buffers;
using System.Text;

namespace EIV_Pack;

public ref partial struct PackReader
{
    private readonly ReadOnlySequence<byte> bufferSource;
    private ReadOnlySpan<byte> currentBuffer;
    private int consumed;
    public readonly Encoding TextEncoding;
    public readonly long Length;
    public readonly int Consumed => consumed;
    public readonly long Remaining => Length - consumed;

    public PackReader(in ReadOnlySequence<byte> sequence)
    {
        bufferSource = sequence;
#if NETSTANDARD
        currentBuffer = sequence.First.Span;
#else
        currentBuffer = sequence.FirstSpan;
#endif
        Length = sequence.Length;
        TextEncoding = Encoding.UTF8;
    }

    public PackReader(ReadOnlySpan<byte> buffer)
    {
        bufferSource = new ReadOnlySequence<byte>(buffer.ToArray());
        currentBuffer = buffer;
        Length = buffer.Length;
        TextEncoding = Encoding.UTF8;
    }

    public void Advance(int count)
    {
        if (count == 0)
            return;

        if (Remaining < count)
        {
            throw new InvalidOperationException("Remaining bytes cannot read this type!");
        }

#if NETSTANDARD
        currentBuffer = bufferSource.Slice(consumed + count, Remaining - count).First.Span;
#else
        currentBuffer = bufferSource.Slice(consumed + count, Remaining - count).FirstSpan;
#endif
        consumed += count;
    }

    public void SetConsumed(int inConsumed)
    {
        if (inConsumed > Length)
            return;

        consumed = inConsumed;

#if NETSTANDARD
        currentBuffer = bufferSource.Slice(consumed, Remaining).First.Span;
#else
        currentBuffer = bufferSource.Slice(consumed, Remaining).FirstSpan;
#endif
    }
}
