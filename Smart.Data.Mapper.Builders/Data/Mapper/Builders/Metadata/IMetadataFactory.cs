namespace Smart.Data.Mapper.Builders.Metadata
{
    using System;

    public interface IMetadataFactory
    {
        TableInfo CreateTableInfo(Type type);
    }
}
