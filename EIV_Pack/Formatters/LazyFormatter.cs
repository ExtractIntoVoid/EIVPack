#if !NETSTANDARD2_0
using System.Diagnostics.CodeAnalysis;
#endif

namespace EIV_Pack.Formatters;

public sealed class LazyFormatter<
#if !NETSTANDARD2_0
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
#endif
    T> : BaseFormatter<Lazy<T?>>
{
    public override void Deserialize(ref PackReader reader, scoped ref Lazy<T?>? value)
    {
        if (!reader.TryReadSmallHeader(out byte count) || count == Constants.SmallNullHeader || count != 1)
        {
            value = null;
            return;
        }

        T? v = reader.ReadValue<T>();
        value = new Lazy<T?>(() => v);
    }

    public override void Serialize(ref PackWriter writer, scoped ref readonly Lazy<T?>? value)
    {
        if (value == null)
        {
            writer.WriteSmallHeader();
            return;
        }

        writer.WriteSmallHeader(1);
        writer.WriteValue(value.Value);
    }
}
