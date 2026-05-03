namespace Smart.Data.Mapper.Handlers;

using System.Data;
using System.Diagnostics.CodeAnalysis;

using Smart.Data.Mapper.Mappers;

public abstract class TypeHandler<T> : ITypeHandler
{
    public abstract void SetValue(IDbDataParameter parameter, T value);

    [return: NotNull]
    public abstract T Parse(object value);

    public void SetValue(IDbDataParameter parameter, object value)
    {
        SetValue(parameter, UnsafeCastHelper.UnsafeCast<T>(value));
    }

    public Func<object, object> CreateParse(Type type)
    {
        return x => Parse(x);
    }
}
