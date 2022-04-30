namespace Smart.Data.Mapper;

using System.Data;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Mocks;

using Xunit;

[System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MethodHasAsyncOverload", Justification = "Ignore")]
public class SqlMapperQueryTest
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

    public async ValueTask QueryAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync().ConfigureAwait(false);
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

        var list = await con.QueryAsync<DataEntity>("SELECT * FROM Data ORDER BY Id").ToListAsync().ConfigureAwait(false);

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

    public async ValueTask QueryCancelAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync().ConfigureAwait(false);
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            var cancel = new CancellationToken(true);
            await con.QueryAsync<DataEntity>(
                    "SELECT * FROM Data ORDER BY Id",
                    cancel: cancel)
                .ToListAsync(cancel)
                .ConfigureAwait(false);
        }).ConfigureAwait(false);
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

    public async ValueTask WithoutOpenMustClosedAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        var list = con.QueryAsync<DataEntity>("SELECT 1, 'test1'");

        Assert.Single(await list.ToListAsync().ConfigureAwait(false));
        Assert.Equal(ConnectionState.Closed, con.State);
    }

    //--------------------------------------------------------------------------------
    // Close
    //--------------------------------------------------------------------------------

    [Fact]

    public void ClosedConnectionMustClosedWhenQueryError()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        Assert.Throws<SqliteException>(() => con.Query<DataEntity>("x").ToList());

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public void ClosedConnectionMustClosedWhenCreateCommandError()
    {
        using var con = new CommandUnsupportedConnection();
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
        Assert.Throws<NotSupportedException>(() => con.Query<DataEntity>(config, "SELECT 1, 'test1'", new object()).ToList());

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async ValueTask ClosedConnectionMustClosedWhenQueryErrorAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await Assert.ThrowsAsync<SqliteException>(async () => await con.QueryAsync<DataEntity>("x").ToListAsync().ConfigureAwait(false)).ConfigureAwait(false);

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async ValueTask ClosedConnectionMustClosedWhenCreateCommandErrorAsync()
    {
#pragma warning disable CA2007
        await using var con = new CommandUnsupportedConnection();
#pragma warning restore CA2007
        await Assert.ThrowsAsync<NotSupportedException>(async () => await con.QueryAsync<DataEntity>("x").ToListAsync().ConfigureAwait(false)).ConfigureAwait(false);

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async ValueTask ClosedConnectionMustClosedWhenPostProcessErrorAsync()
    {
        var config = new SqlMapperConfig();
        config.ConfigureParameterBuilderFactories(opt =>
        {
            opt.Clear();
            opt.Add(new PostProcessErrorParameterBuilderFactory());
        });

#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await Assert.ThrowsAsync<NotSupportedException>(async () => await con.QueryAsync<DataEntity>(config, "SELECT 1, 'test1'", new object()).ToListAsync().ConfigureAwait(false)).ConfigureAwait(false);

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

    public async ValueTask ProcessParameterAsync()
    {
        var factory = new MockParameterBuilderFactory();
        var config = new SqlMapperConfig();
        config.ConfigureParameterBuilderFactories(opt =>
        {
            opt.Clear();
            opt.Add(factory);
        });

#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        var list = con.QueryAsync<DataEntity>(config, "SELECT 1, 'test1'", new object());

        Assert.Single(await list.ToListAsync().ConfigureAwait(false));
        Assert.True(factory.BuildCalled);
        Assert.True(factory.PostProcessCalled);
    }

    [Fact]

    public async ValueTask ProcessParameterIsNothingAsync()
    {
        var factory = new MockParameterBuilderFactory();
        var config = new SqlMapperConfig();
        config.ConfigureParameterBuilderFactories(opt =>
        {
            opt.Clear();
            opt.Add(factory);
        });

#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        var list = con.QueryAsync<DataEntity>(config, "SELECT 1, 'test1'");

        Assert.Single(await list.ToListAsync().ConfigureAwait(false));
        Assert.False(factory.BuildCalled);
        Assert.False(factory.PostProcessCalled);
    }

    protected class DataEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
