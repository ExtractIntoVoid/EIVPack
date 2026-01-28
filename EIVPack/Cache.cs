using EIVPack.Formatters;

namespace EIVPack;

public static class Cache<T>
{
    public static bool IsRegistered { get; internal set; }

    public static IFormatter<T>? Formatter { get; internal set; }
}
