namespace Smart.Data.Mapper;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Mocks;

public sealed class TypeHandlerTest
{
    [Fact]

    public void ExecuteScalarByObjectParameter()
    {
        SqlMapperConfig.Default.ConfigureTypeHandlers(static config =>
        {
            config[typeof(DateTime)] = new DateTimeTypeHandler();
        });

        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Date int)");

        var date = new DateTime(2000, 1, 1);
        con.Execute("INSERT INTO Data (Id, Date) VALUES (@Id, @Date)", new DataEntity { Id = 1, Date = date });

        var entity = con.QueryFirstOrDefault<DataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 1 });

        Assert.Equal(date, entity!.Date);

        var rawValue = con.ExecuteScalar<long>("SELECT Date FROM Data WHERE Id = @Id", new { Id = 1 });

        Assert.Equal(date.Ticks, rawValue);
    }

    public sealed class DataEntity
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }
    }
}
