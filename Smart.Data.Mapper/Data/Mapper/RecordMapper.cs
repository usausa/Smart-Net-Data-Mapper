namespace Smart.Data.Mapper;

using System.Data;

public abstract class RecordMapper<T>
{
    public abstract T Map(IDataRecord record);
}
