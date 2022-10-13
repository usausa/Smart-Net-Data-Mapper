namespace Smart.Data.Mapper;

using System.Data;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Mocks;

using Xunit;

[System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MethodHasAsyncOverload", Justification = "Ignore")]
public class SqlMapperQueryFirstOrDefaultTest
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
        Assert.Equal(1, entity!.Id);
        Assert.Equal("test1", entity.Name);

        entity = con.QueryFirstOrDefault<DataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 0 });

        Assert.Null(entity);
    }

    [Fact]

    public async Task QueryFirstOrDefaultAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync().ConfigureAwait(false);
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

        var entity = await con.QueryFirstOrDefaultAsync<DataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 1 }).ConfigureAwait(false);

        Assert.NotNull(entity);
        Assert.Equal(1, entity!.Id);
        Assert.Equal("test1", entity.Name);

        entity = await con.QueryFirstOrDefaultAsync<DataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 0 }).ConfigureAwait(false);

        Assert.Null(entity);
    }

    //--------------------------------------------------------------------------------
    // Cancel
    //--------------------------------------------------------------------------------

    [Fact]

    public async Task QueryFirstOrDefaultCancelAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync().ConfigureAwait(false);

        var cancel = new CancellationToken(true);
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                await con.QueryFirstOrDefaultAsync<DataEntity>(
                    "SELECT * FROM Data WHERE Id = @Id",
                    new { Id = 1 },
                    cancel: cancel).ConfigureAwait(false))
            .ConfigureAwait(false);
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
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.QueryFirstOrDefaultAsync<DataEntity>("SELECT 1, 'test1'").ConfigureAwait(false);

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    //--------------------------------------------------------------------------------
    // Close
    //--------------------------------------------------------------------------------

    [Fact]

    public void ClosedConnectionMustClosedWhenQueryError()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        Assert.Throws<SqliteException>(() => con.QueryFirstOrDefault<DataEntity>("x"));

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task ClosedConnectionMustClosedWhenQueryErrorAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await Assert.ThrowsAsync<SqliteException>(async () => await con.QueryFirstOrDefaultAsync<DataEntity>("x").ConfigureAwait(false)).ConfigureAwait(false);

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

#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.QueryFirstOrDefaultAsync<DataEntity>(config, "SELECT 1, 'test1'", new object()).ConfigureAwait(false);

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

#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.QueryFirstOrDefaultAsync<DataEntity>(config, "SELECT 1, 'test1'").ConfigureAwait(false);

        Assert.False(factory.BuildCalled);
        Assert.False(factory.PostProcessCalled);
    }

    protected class DataEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }
}
