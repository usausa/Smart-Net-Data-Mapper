namespace Smart.Data.Mapper.Mappers;

using Smart.Data.Mapper.Attributes;
using Smart.Mock.Data;

public sealed class ObjectResultMapperFactoryTest
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

#pragma warning disable CA2000
        var reader = new MockDataReader(columns, values);
#pragma warning restore CA2000
        con.SetupCommand(cmd => cmd.SetupResult(reader));

        var config = new SqlMapperConfig();
        config.ConfigureResultMapperFactories(c =>
        {
            c.Clear();
            c.Add(ObjectResultMapperFactory.Instance);
        });
        var list = con.Query<DataEntity>(config, "SELECT * FROM Data").ToList();

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

    private sealed class DataEntity
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

#pragma warning disable SA1602
    public enum Value
    {
        Zero = 0,
        One = 1
    }
#pragma warning restore SA1602
}
