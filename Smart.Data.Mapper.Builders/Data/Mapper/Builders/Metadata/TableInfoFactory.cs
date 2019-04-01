namespace Smart.Data.Mapper.Builders.Metadata
{
    public static class TableInfoFactory
    {
        public static ITableInfoProvider Provider { get; set; } = StandardTableInfoProvider.Default;
    }
}
