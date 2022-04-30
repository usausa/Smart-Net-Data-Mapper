namespace Smart.Data.Mapper.Builders.Metadata;

public interface ITableMetadataProvider
{
    TableMetadata Create(Type type);
}
