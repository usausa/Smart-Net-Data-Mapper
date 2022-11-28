namespace Smart.Data.Mapper.Mappers;

public interface IResultMapperFactory
{
    bool IsMatch(Type type);

    ResultMapper<T> CreateMapper<T>(ISqlMapperConfig config, Type type, ColumnInfo[] columns);
}
