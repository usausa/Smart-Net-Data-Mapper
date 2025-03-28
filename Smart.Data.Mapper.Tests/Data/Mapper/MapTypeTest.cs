namespace Smart.Data.Mapper;

using Microsoft.Data.Sqlite;

public sealed class MapTypeTest
{
    //--------------------------------------------------------------------------------
    // Record
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapRecord()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

        var entity = con.QueryFirstOrDefault<RecordEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 1 });

        Assert.NotNull(entity);
        Assert.Equal(1, entity.Id);
        Assert.Equal("test1", entity.Name);
    }

    public sealed record RecordEntity
    {
        public long Id { get; set; }

        public string? Name { get; set; }
    }

    //--------------------------------------------------------------------------------
    // Record
    //--------------------------------------------------------------------------------

    [Fact]
    public void MapInitOnly()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

        var entity = con.QueryFirstOrDefault<InitOnlyEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 1 });

        Assert.NotNull(entity);
        Assert.Equal(1, entity.Id);
        Assert.Equal("test1", entity.Name);
    }

    private sealed class InitOnlyEntity
    {
        public long Id { get; init; }

        public string? Name { get; init; }
    }
}
