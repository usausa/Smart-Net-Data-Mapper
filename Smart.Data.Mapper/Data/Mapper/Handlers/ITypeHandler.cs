namespace Smart.Data.Mapper.Handlers
{
    using System;
    using System.Data;

    public interface ITypeHandler
    {
        void SetValue(IDbDataParameter parameter, object value);

        Func<object, object> CreateParse(Type type);
    }
}
