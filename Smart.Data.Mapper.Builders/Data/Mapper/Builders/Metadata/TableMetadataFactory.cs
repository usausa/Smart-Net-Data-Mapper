namespace Smart.Data.Mapper.Builders.Metadata;

public static class TableMetadataFactory
{
    public static ITableMetadataProvider Provider { get; set; } = StandardTableInfoProvider.Default;
}
