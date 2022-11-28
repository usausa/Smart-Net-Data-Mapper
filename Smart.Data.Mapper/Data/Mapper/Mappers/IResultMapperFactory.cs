namespace Smart.Data.Mapper.Mappers;

using System.Data;

public interface IResultMapperFactory
{
    bool IsMatch(Type type);

    RecordMapper<T> CreateMapper<T>(ISqlMapperConfig config, Type type, ColumnInfo[] columns);
}
