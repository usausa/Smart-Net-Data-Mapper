namespace Smart.Data.Mapper;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Attributes;

public sealed class PropertySelectorTest
{
    [Fact]
    public void MapProperty()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        var entity = con.QueryFirstOrDefault<DataEntity>(
            "SELECT " +
            "1 AS ColumnName1, " +
            "2 AS columnName2, " +
            "3 AS column_name3, " +
            "4 AS COLUMN_NAME4, " +
            "5 AS column_name5, " +
            "6 AS ColumnName6, " +
            "7 AS ColumnName7");

        Assert.NotNull(entity);
        Assert.Equal(1, entity.ColumnName1);
        Assert.Equal(2, entity.ColumnName2);
        Assert.Equal(3, entity.ColumnName3);
        Assert.Equal(4, entity.ColumnName4);
        Assert.Equal(5, entity.ColumnName5);
        Assert.Equal(0, entity.ColumnName6);
    }

    public sealed class DataEntity
    {
        public int ColumnName1 { get; set; }

        public int ColumnName2 { get; set; }

        public int ColumnName3 { get; set; }

        [Name("COLUMN_NAME4")]
        public int ColumnName4 { get; set; }

        [Name("COLUMN_NAME5")]
        public int ColumnName5 { get; set; }

        [Ignore]
        public int ColumnName6 { get; set; }
    }
}
