namespace Smart.Data.Mapper.Builders.Metadata;

using System.Diagnostics.CodeAnalysis;

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

#pragma warning disable CA1000
public static class TableInfo<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] T>
{
    public static TableMetadata Instance { get; }

    static TableInfo()
    {
        Instance = TableMetadataFactory.Provider.Create(typeof(T));
    }
}
#pragma warning restore CA1000
