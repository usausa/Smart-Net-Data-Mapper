namespace Smart.Data.Mapper.Builders.Metadata
{
    using System;

    public interface ITableInfoProvider
    {
        TableInfo Create(Type type);
    }
}
