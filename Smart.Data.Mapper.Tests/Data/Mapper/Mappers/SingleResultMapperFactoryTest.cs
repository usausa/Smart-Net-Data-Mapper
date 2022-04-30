namespace Smart.Data.Mapper.Mappers;

using Microsoft.Data.Sqlite;

using Xunit;

public class SingleResultMapperFactoryTest
{
    //--------------------------------------------------------------------------------
    // Query
    //--------------------------------------------------------------------------------

    [Fact]

    public void QueryList()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

        var list = con.QueryList<int>("SELECT Id FROM Data ORDER BY Id").ToList();

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
    }

    protected class DataEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
