namespace Smart.Data.Mapper.Mappers;

using System.Data;

public abstract class RecordMapper<T>
{
    public abstract T Map(IDataRecord record);
}
