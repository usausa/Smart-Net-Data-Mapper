// ReSharper disable MethodHasAsyncOverload
namespace Smart.Data.Mapper;

using System.Data;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Mocks;

#pragma warning disable xUnit1051
public sealed class SqlMapperQueryFirstOrDefaultTest
{
    //--------------------------------------------------------------------------------
    // Query
    //--------------------------------------------------------------------------------

    [Fact]

    public void QueryFirstOrDefault()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

        var entity = con.QueryFirstOrDefault<DataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 1 });

        Assert.NotNull(entity);
        Assert.Equal(1, entity.Id);
        Assert.Equal("test1", entity.Name);

        entity = con.QueryFirstOrDefault<DataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 0 });

        Assert.Null(entity);
    }

    [Fact]

    public async Task QueryFirstOrDefaultAsync()
    {
        await using var con = new SqliteConnection("Data Source=:memory:");
        await con.OpenAsync();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

        var entity = await con.QueryFirstOrDefaultAsync<DataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 1 });

        Assert.NotNull(entity);
        Assert.Equal(1, entity.Id);
        Assert.Equal("test1", entity.Name);

        entity = await con.QueryFirstOrDefaultAsync<DataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 0 });

        Assert.Null(entity);
    }

    //--------------------------------------------------------------------------------
    // Cancel
    //--------------------------------------------------------------------------------

    [Fact]

    public async Task QueryFirstOrDefaultCancelAsync()
    {
        await using var con = new SqliteConnection("Data Source=:memory:");
        await con.OpenAsync();

        var cancel = new CancellationToken(true);
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await con.QueryFirstOrDefaultAsync<DataEntity>(
                    "SELECT * FROM Data WHERE Id = @Id",
                    new { Id = 1 },
                    cancel: cancel))
            ;
    }

    //--------------------------------------------------------------------------------
    // Open
    //--------------------------------------------------------------------------------

    [Fact]

    public void WithoutOpen()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        con.QueryFirstOrDefault<DataEntity>("SELECT 1, 'test1'");

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task WithoutOpenAsync()
    {
        await using var con = new SqliteConnection("Data Source=:memory:");
        await con.QueryFirstOrDefaultAsync<DataEntity>("SELECT 1, 'test1'");

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    //--------------------------------------------------------------------------------
    // Close
    //--------------------------------------------------------------------------------

    [Fact]

    public void ClosedConnectionMustClosedWhenQueryError()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        // ReSharper disable once AccessToDisposedClosure
        Assert.Throws<SqliteException>(() => con.QueryFirstOrDefault<DataEntity>("x"));

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task ClosedConnectionMustClosedWhenQueryErrorAsync()
    {
        await using var con = new SqliteConnection("Data Source=:memory:");
        await Assert.ThrowsAsync<SqliteException>(async () => await con.QueryFirstOrDefaultAsync<DataEntity>("x"));

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    //--------------------------------------------------------------------------------
    // Parameter
    //--------------------------------------------------------------------------------

    [Fact]

    public void ProcessParameter()
    {
        var factory = new MockParameterBuilderFactory();
        var config = new SqlMapperConfig();
        config.ConfigureParameterBuilderFactories(opt =>
        {
            opt.Clear();
            opt.Add(factory);
        });

        using var con = new SqliteConnection("Data Source=:memory:");
        con.QueryFirstOrDefault<DataEntity>(config, "SELECT 1, 'test1'", new object());

        Assert.True(factory.BuildCalled);
        Assert.True(factory.PostProcessCalled);
    }

    [Fact]

    public void ProcessParameterIsNothing()
    {
        var factory = new MockParameterBuilderFactory();
        var config = new SqlMapperConfig();
        config.ConfigureParameterBuilderFactories(opt =>
        {
            opt.Clear();
            opt.Add(factory);
        });

        using var con = new SqliteConnection("Data Source=:memory:");
        con.QueryFirstOrDefault<DataEntity>(config, "SELECT 1, 'test1'");

        Assert.False(factory.BuildCalled);
        Assert.False(factory.PostProcessCalled);
    }

    [Fact]

    public async Task ProcessParameterAsync()
    {
        var factory = new MockParameterBuilderFactory();
        var config = new SqlMapperConfig();
        config.ConfigureParameterBuilderFactories(opt =>
        {
            opt.Clear();
            opt.Add(factory);
        });

        await using var con = new SqliteConnection("Data Source=:memory:");
        await con.QueryFirstOrDefaultAsync<DataEntity>(config, "SELECT 1, 'test1'", new object());

        Assert.True(factory.BuildCalled);
        Assert.True(factory.PostProcessCalled);
    }

    [Fact]

    public async Task ProcessParameterIsNothingAsync()
    {
        var factory = new MockParameterBuilderFactory();
        var config = new SqlMapperConfig();
        config.ConfigureParameterBuilderFactories(opt =>
        {
            opt.Clear();
            opt.Add(factory);
        });

        await using var con = new SqliteConnection("Data Source=:memory:");
        await con.QueryFirstOrDefaultAsync<DataEntity>(config, "SELECT 1, 'test1'");

        Assert.False(factory.BuildCalled);
        Assert.False(factory.PostProcessCalled);
    }

    private sealed class DataEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
