namespace Smart.Data.Mapper.Builders.Metadata;

public sealed class TableMetadata
{
    public string Name { get; }

    public IReadOnlyList<ColumnMetadata> Columns { get; }

    public IReadOnlyList<ColumnMetadata> KeyColumns { get; }

    public IReadOnlyList<ColumnMetadata> NonKeyColumns { get; }

    public TableMetadata(string name, IReadOnlyList<ColumnMetadata> columns, IReadOnlyList<ColumnMetadata> keyColumns, IReadOnlyList<ColumnMetadata> nonKeyColumns)
    {
        Name = name;
        Columns = columns;
        KeyColumns = keyColumns;
        NonKeyColumns = nonKeyColumns;
    }
}

public static class TableInfo<T>
{
    public static TableMetadata Instance { get; }

    static TableInfo()
    {
        Instance = TableMetadataFactory.Provider.Create(typeof(T));
    }
}
