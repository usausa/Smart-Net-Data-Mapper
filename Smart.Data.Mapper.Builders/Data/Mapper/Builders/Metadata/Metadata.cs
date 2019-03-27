namespace Smart.Data.Mapper.Builders.Metadata
{
    public static class Metadata
    {
        public static IMetadataFactory Factory { get; } = StandardMetadataFactory.Default;
    }

    public static class Metadata<T>
    {
        public static TableInfo Table { get; }

        static Metadata()
        {
            Table = Metadata.Factory.CreateTableInfo(typeof(T));
        }
    }
}
