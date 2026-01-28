using Microsoft.IO;
using System.Text;

namespace EIVPack;

public ref partial struct PackWriter(Encoding encoding) : IDisposable
{
    private readonly RecyclableMemoryStream recyclable = Constants.StreamManager.GetStream();
    public readonly Encoding TextEncoding = encoding;
    private int depth = 0;
    private const int DepthLimit = 1000;

    public PackWriter() : this(Encoding.UTF8) { }

    public readonly void Dispose()
    {
        recyclable.Dispose();
    }
}
