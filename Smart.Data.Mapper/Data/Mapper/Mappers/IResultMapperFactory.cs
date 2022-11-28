namespace Smart.Data.Mapper.Mappers;

public interface IResultMapperFactory
{
    bool IsMatch(Type type);

    RecordMapper<T> CreateMapper<T>(ISqlMapperConfig config, Type type, ColumnInfo[] columns);
}
