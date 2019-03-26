namespace Smart.Data.Mapper.Builder.Metadata
{
    public static class Metadata
    {
        public static IMetadataFactory Factory { get; } = DefaultMetadataFactory.Instance;
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
