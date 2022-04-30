namespace Smart.Data.Mapper;

using Smart.Data.Mapper.Attributes;
using Smart.Mock.Data;

using Xunit;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
public class ObjectResultMapperFactoryTest
{
    [Fact]
    public void MapProperty()
    {
        using var con = new MockDbConnection();
        var columns = new[]
        {
            new MockColumn(typeof(int), "Column1"),
            new MockColumn(typeof(int), "Column2"),
            new MockColumn(typeof(int), "Column3"),
            new MockColumn(typeof(int), "Column4"),
            new MockColumn(typeof(int), "Column5"),
            new MockColumn(typeof(int), "Column6"),
            new MockColumn(typeof(int), "Column7"),
            new MockColumn(typeof(int), "Column8")
        };
        var values = new List<object[]>
        {
            new object[] { 1, 1, 1, 1, 1, 1, 1, 1 },
            new object[] { DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value }
        };

        var reader = new MockDataReader(columns, values);
        con.SetupCommand(cmd => cmd.SetupResult(reader));

        var list = con.Query<DataEntity>("SELECT * FROM Data").ToList();

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0].Column1);
        Assert.Equal(1, list[0].Column2);
        Assert.Equal(1, list[0].Column3);
        Assert.Equal(Value.One, list[0].Column4);
        Assert.Equal(Value.One, list[0].Column5);
        Assert.Equal(0, list[0].Column6);
        Assert.Equal(0, list[0].Column7);
        Assert.Equal(0, list[1].Column1);
        Assert.Null(list[1].Column2);
        Assert.Equal(0, list[1].Column3);
        Assert.Equal(Value.Zero, list[1].Column4);
        Assert.Null(list[1].Column5);
        Assert.Equal(0, list[1].Column6);
        Assert.Equal(0, list[1].Column7);
    }

    protected class DataEntity
    {
        public int Column1 { get; set; }

        public int? Column2 { get; set; }

        public long Column3 { get; set; }

        public Value Column4 { get; set; }

        public Value? Column5 { get; set; }

        public int Column6 => Column7;

        [Ignore]
        public int Column7 { get; set; }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:EnumerationItemsMustBeDocumented", Justification = "Ignore")]
    protected enum Value
    {
        Zero = 0,
        One = 1
    }
}
