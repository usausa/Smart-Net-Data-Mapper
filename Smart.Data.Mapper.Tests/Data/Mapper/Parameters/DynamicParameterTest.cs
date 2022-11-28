namespace Smart.Data.Mapper.Parameters;

using System.Data;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Mocks;
using Smart.Mock.Data;

using Xunit;

public class DynamicParameterTest
{
    [Fact]

    public void Parameters()
    {
        using var con = new MockDbConnection();
        con.SetupCommand(c => c.SetupResult(0));

        var parameter = new DynamicParameter();
        parameter.Add("Parameter1", 1, direction: ParameterDirection.Output);
        parameter.Add("Parameter2", null);
        parameter.Add("Parameter3", 3, DbType.Int16);
        parameter.Add("Parameter4", "4", size: 1);

        con.Execute("PROC", parameter, commandType: CommandType.StoredProcedure);

        var cmd = con.Commands[0];
        var param1 = (IDbDataParameter)cmd.Parameters[0];
        Assert.Equal(ParameterDirection.Output, param1.Direction);
        var param2 = (IDbDataParameter)cmd.Parameters[1];
        Assert.Equal(DBNull.Value, param2.Value);
        var param3 = (IDbDataParameter)cmd.Parameters[2];
        Assert.Equal(DbType.Int16, param3.DbType);
        var param4 = (IDbDataParameter)cmd.Parameters[3];
        Assert.Equal(1, param4.Size);
    }

    [Fact]

    public void ParameterGet()
    {
        using var con = new MockDbConnection();
        con.SetupCommand(c => c.SetupResult(0));

        var parameter = new DynamicParameter();
        parameter.Add("Integer1", 1);
        parameter.Add("Integer2", null);
        parameter.Add("String1", "a");
        parameter.Add("String2", null);

        con.Execute("PROC", parameter, commandType: CommandType.StoredProcedure);

        Assert.Equal(1, parameter.Get<int>("Integer1"));
        Assert.Equal(0, parameter.Get<int>("Integer2"));
        Assert.Equal("a", parameter.Get<string>("String1"));
        Assert.Null(parameter.Get<string>("String2"));
    }

    [Fact]

    public void ParameterByDynamicParameter()
    {
        SqlMapperConfig.Default.ConfigureTypeHandlers(config =>
        {
            config[typeof(DateTime)] = new DateTimeTypeHandler();
        });

        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text, Date int)");

        var parameter = new DynamicParameter();
        parameter.Add("Id", 1);
        parameter.Add("Name", null, DbType.StringFixedLength, 10);
        parameter.Add("Date", new DateTime(2000, 1, 1));
        con.Execute("INSERT INTO Data (Id, Name, Date) VALUES (@Id, @Name, @Date)", parameter);

        var entity = con.QueryFirstOrDefault<DataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 1 });

        Assert.Equal(1L, entity!.Id);
        Assert.Null(entity.Name);
        Assert.Equal(new DateTime(2000, 1, 1), entity.Date);
    }

    protected class DataEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public DateTime Date { get; set; }
    }
}
