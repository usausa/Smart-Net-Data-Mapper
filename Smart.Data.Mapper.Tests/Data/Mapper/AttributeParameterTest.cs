namespace Smart.Data.Mapper;

using System.Data;

using Smart.Data.Mapper.Attributes;
using Smart.Mock.Data;

public sealed class AttributeParameterTest
{
    [Fact]

    public void ParameterByAttribute()
    {
        using var con = new MockDbConnection();
        con.SetupCommand(static cmd => cmd.SetupResult(0));

        var parameter = new Parameter { Value = 1 };
        con.Execute("PROC", parameter, commandType: CommandType.StoredProcedure);

        var cmd = con.Commands[0];
        Assert.Single(cmd.Parameters);

        var param = (MockDbParameter)cmd.Parameters[0];
        Assert.Equal(DbType.Int64, param.DbType);
        Assert.Equal(10, param.Size);
        Assert.Equal(ParameterDirection.InputOutput, param.Direction);
    }

    private sealed class Parameter
    {
        [DbType(DbType.Int64)]
        [Size(10)]
        [Direction(ParameterDirection.InputOutput)]
        public int Value { get; set; }

        [Ignore]
        public string? Ignore { get; set; }
    }
}
