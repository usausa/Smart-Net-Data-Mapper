// ReSharper disable MethodHasAsyncOverload
namespace Smart.Data.Mapper;

using System.Data;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Mocks;

#pragma warning disable xUnit1051
public sealed class SqlMapperQueryTest
{
    //--------------------------------------------------------------------------------
    // Query
    //--------------------------------------------------------------------------------

    [Fact]

    public void Query()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

        var list = con.Query<DataEntity>("SELECT * FROM Data ORDER BY Id").ToList();

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0].Id);
        Assert.Equal("test1", list[0].Name);
        Assert.Equal(2, list[1].Id);
        Assert.Equal("test2", list[1].Name);
    }

    [Fact]

    public async Task QueryAsync()
    {
        await using var con = new SqliteConnection("Data Source=:memory:");
        await con.OpenAsync();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

        var list = await con.QueryAsync<DataEntity>("SELECT * FROM Data ORDER BY Id").ToListAsync();

        Assert.Equal(2, list.Count);
        Assert.Equal(1, list[0].Id);
        Assert.Equal("test1", list[0].Name);
        Assert.Equal(2, list[1].Id);
        Assert.Equal("test2", list[1].Name);
    }

    //--------------------------------------------------------------------------------
    // Cancel
    //--------------------------------------------------------------------------------

    [Fact]

    public async Task QueryCancelAsync()
    {
        await using var con = new SqliteConnection("Data Source=:memory:");
        await con.OpenAsync();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            var cancel = new CancellationToken(true);
            await con.QueryAsync<DataEntity>(
                    "SELECT * FROM Data ORDER BY Id",
                    cancel: cancel)
                .ToListAsync(cancel)
                ;
        });
    }

    //--------------------------------------------------------------------------------
    // Open
    //--------------------------------------------------------------------------------

    [Fact]

    public void WithoutOpenMustClosed()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        var list = con.Query<DataEntity>("SELECT 1, 'test1'");

        Assert.Single(list.ToList());
        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task WithoutOpenMustClosedAsync()
    {
        await using var con = new SqliteConnection("Data Source=:memory:");
        var list = con.QueryAsync<DataEntity>("SELECT 1, 'test1'");

        Assert.Single(await list.ToListAsync());
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
        Assert.Throws<SqliteException>(() => con.Query<DataEntity>("x").ToList());

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public void ClosedConnectionMustClosedWhenCreateCommandError()
    {
        using var con = new CommandUnsupportedConnection();
        // ReSharper disable once AccessToDisposedClosure
        Assert.Throws<NotSupportedException>(() => con.Query<DataEntity>("x").ToList());

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public void ClosedConnectionMustClosedWhenPostProcessError()
    {
        var config = new SqlMapperConfig();
        config.ConfigureParameterBuilderFactories(opt =>
        {
            opt.Clear();
            opt.Add(new PostProcessErrorParameterBuilderFactory());
        });

        using var con = new SqliteConnection("Data Source=:memory:");
        // ReSharper disable once AccessToDisposedClosure
        Assert.Throws<NotSupportedException>(() => con.Query<DataEntity>(config, "SELECT 1, 'test1'", new object()).ToList());

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task ClosedConnectionMustClosedWhenQueryErrorAsync()
    {
        await using var con = new SqliteConnection("Data Source=:memory:");
        await Assert.ThrowsAsync<SqliteException>(async () => await con.QueryAsync<DataEntity>("x").ToListAsync());

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task ClosedConnectionMustClosedWhenCreateCommandErrorAsync()
    {
        await using var con = new CommandUnsupportedConnection();
        await Assert.ThrowsAsync<NotSupportedException>(async () => await con.QueryAsync<DataEntity>("x").ToListAsync());

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task ClosedConnectionMustClosedWhenPostProcessErrorAsync()
    {
        var config = new SqlMapperConfig();
        config.ConfigureParameterBuilderFactories(opt =>
        {
            opt.Clear();
            opt.Add(new PostProcessErrorParameterBuilderFactory());
        });

        await using var con = new SqliteConnection("Data Source=:memory:");
        await Assert.ThrowsAsync<NotSupportedException>(async () => await con.QueryAsync<DataEntity>(config, "SELECT 1, 'test1'", new object()).ToListAsync());

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
        var list = con.Query<DataEntity>(config, "SELECT 1, 'test1'", new object());

        Assert.Single(list.ToList());
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
        var list = con.Query<DataEntity>(config, "SELECT 1, 'test1'");

        Assert.Single(list.ToList());
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
        var list = con.QueryAsync<DataEntity>(config, "SELECT 1, 'test1'", new object());

        Assert.Single(await list.ToListAsync());
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
        var list = con.QueryAsync<DataEntity>(config, "SELECT 1, 'test1'");

        Assert.Single(await list.ToListAsync());
        Assert.False(factory.BuildCalled);
        Assert.False(factory.PostProcessCalled);
    }

    private sealed class DataEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
