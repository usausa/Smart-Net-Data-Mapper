namespace Smart.Data.Mapper.Builder.Metadata
{
    using System;

    public interface IMetadataFactory
    {
        TableInfo CreateTableInfo(Type type);
    }
}
