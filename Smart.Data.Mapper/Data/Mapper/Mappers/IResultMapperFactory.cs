namespace Smart.Data.Mapper.Mappers
{
    using System;
    using System.Data;

    public interface IResultMapperFactory
    {
        bool IsMatch(Type type);

        Func<IDataRecord, T> CreateMapper<T>(ISqlMapperConfig config, Type type, ColumnInfo[] columns);
    }
}
