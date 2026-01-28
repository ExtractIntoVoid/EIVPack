using EIVPack.Formatters;
using System.Numerics;
using System.Text;

namespace EIVPack;

public static partial class FormatterProvider
{
    static FormatterProvider()
    {
        RegisterFormatters();
    }

    public static bool IsRegistered<T>()
    {
        return Cache<T>.IsRegistered;
    }

    public static void Register<T>(IFormatter<T> formatter)
    {
        Cache<T>.IsRegistered = true;
        Cache<T>.Formatter = formatter;
    }

    public static void Register<T>() where T : IFormatterRegister
    {
        T.RegisterFormatter();
    }

    public static IFormatter<T> GetFormatter<T>()
    {
        if (!IsRegistered<T>())
            PackException.ThrowNotRegisteredInProvider(typeof(T));

        var formatter = Cache<T>.Formatter;
        if (formatter == null)
            PackException.ThrowNotRegisteredInProvider(typeof(T));

        return formatter;
    }

    private static void RegisterFormatters()
    {
        RegisterToAll<SByte>();
        RegisterToAll<Byte>();
        RegisterToAll<Int16>();
        RegisterToAll<UInt16>();
        RegisterToAll<Int32>();
        RegisterToAll<UInt32>();
        RegisterToAll<Int64>();
        RegisterToAll<UInt64>();
        RegisterToAll<UInt128>();
        RegisterToAll<Int128>();
        RegisterToAll<Char>();
        RegisterToAll<Single>();
        RegisterToAll<Double>();
        RegisterToAll<Decimal>();
        RegisterToAll<Boolean>();
        RegisterToAll<IntPtr>();
        RegisterToAll<UIntPtr>();
        RegisterToAll<Rune>();
        RegisterToAll<DateTime>();
        RegisterToAll<DateTimeOffset>();
        RegisterToAll<TimeSpan>();
        RegisterToAll<Guid>();
        RegisterToAll<DateOnly>();
        RegisterToAll<TimeOnly>();
        RegisterToAll<Complex>();
        RegisterToAll<Matrix3x2>();
        RegisterToAll<Matrix4x4>();
        RegisterToAll<Plane>();
        RegisterToAll<Quaternion>();
        RegisterToAll<Vector2>();
        RegisterToAll<Vector3>();
        RegisterToAll<Vector4>();
    }

    private static void RegisterToAll<T>() where T : unmanaged
    {
        Register(new UnmanagedFormatter<T>());
        Register(new NullableFormatter<T>());
        Register(new ArrayFormatter<T>());
        Register(new ArraySegmentFormatter<T>());
        Register(new MemoryFormatter<T>());
        Register(new ReadOnlyMemoryFormatter<T>());
    }
}
