namespace Smart.Data.Mapper;

using System.Data;
using System.Reflection;

using Smart.Data.Mapper.Parameters;

public interface ISqlMapperConfig
{
    Func<T> CreateFactory<T>();

    Func<object?, object?>? CreateGetter(PropertyInfo pi);

    Action<object?, object?>? CreateSetter(PropertyInfo pi);

    T Convert<T>(object source);

    Func<PropertyInfo[], string, PropertyInfo?> GetPropertySelector();

    Func<object, object>? CreateParser(Type sourceType, Type destinationType);

    TypeHandleEntry LookupTypeHandle(Type type);

    ParameterBuilder CreateParameterBuilder(Type type);

    Func<IDataRecord, T> CreateResultMapper<T>(IDataReader reader);
}
