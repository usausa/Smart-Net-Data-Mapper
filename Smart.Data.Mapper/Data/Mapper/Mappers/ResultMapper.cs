namespace Smart.Data.Mapper.Mappers;

using System.Data;

public abstract class ResultMapper<T>
{
    public abstract T Map(IDataRecord record);
}
