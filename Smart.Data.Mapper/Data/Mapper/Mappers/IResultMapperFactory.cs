namespace Smart.Data.Mapper.Mappers;

using System.Diagnostics.CodeAnalysis;

public interface IResultMapperFactory
{
    bool IsMatch(Type type);

    ResultMapper<T> CreateMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(ISqlMapperConfig config, Type type, ColumnInfo[] columns);
}
