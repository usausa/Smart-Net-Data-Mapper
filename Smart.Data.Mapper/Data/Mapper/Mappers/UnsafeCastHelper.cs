namespace Smart.Data.Mapper.Mappers;

using System.Runtime.CompilerServices;

internal static class UnsafeCastHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static T UnsafeCast<T>(object obj)
    {
        if (typeof(T).IsValueType)
        {
            return (T)obj;
        }

        return Unsafe.As<object, T>(ref obj);
    }
}
