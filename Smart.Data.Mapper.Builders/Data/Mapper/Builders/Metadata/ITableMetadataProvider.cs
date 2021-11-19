namespace Smart.Data.Mapper.Builders.Metadata;

using System;

public interface ITableMetadataProvider
{
    TableMetadata Create(Type type);
}
