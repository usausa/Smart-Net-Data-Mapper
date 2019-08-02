namespace Smart.Data.Mapper.Handlers
{
    using System;
    using System.Data;

    public abstract class TypeHandler<T> : ITypeHandler
    {
        public abstract void SetValue(IDbDataParameter parameter, T value);

        public abstract T Parse(object value);

        public void SetValue(IDbDataParameter parameter, object value)
        {
            SetValue(parameter, (T)value);
        }

        public Func<object, object> CreateParse(Type type)
        {
            return x => Parse(x);
        }
    }
}
