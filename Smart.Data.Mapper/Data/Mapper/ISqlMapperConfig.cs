namespace Smart.Data.Mapper;

using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using Smart.Data.Mapper.Mappers;
using Smart.Data.Mapper.Parameters;

public interface ISqlMapperConfig
{
    Func<T> CreateFactory<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicConstructors)] T>();

    Func<object?, object?>? CreateGetter(PropertyInfo pi);

    Action<object?, object?>? CreateSetter(PropertyInfo pi);

    T Convert<T>(object source);

    Func<PropertyInfo[], string, PropertyInfo?> GetPropertySelector();

    Func<object, object>? CreateParser(Type sourceType, Type destinationType);

    TypeHandleEntry LookupTypeHandle(Type type);

    ParameterBuilder CreateParameterBuilder([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicConstructors)] Type type);

    ResultMapper<T> CreateResultMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(IDataReader reader);
}
