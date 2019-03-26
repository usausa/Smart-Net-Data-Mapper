namespace Smart.Data.Mapper.Builders.Metadata
{
    using System.Collections.Generic;

    public sealed class TableInfo
    {
        public string Name { get; }

        public IReadOnlyList<ColumnInfo> Columns { get; }

        public IReadOnlyList<ColumnInfo> KeyColumns { get; }

        public IReadOnlyList<ColumnInfo> NonKeyColumns { get; }

        public TableInfo(string name, IReadOnlyList<ColumnInfo> columns, IReadOnlyList<ColumnInfo> keyColumns, IReadOnlyList<ColumnInfo> nonKeyColumns)
        {
            Name = name;
            Columns = columns;
            KeyColumns = keyColumns;
            NonKeyColumns = nonKeyColumns;
        }
    }
}
