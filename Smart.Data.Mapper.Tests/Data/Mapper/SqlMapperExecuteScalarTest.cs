// ReSharper disable MethodHasAsyncOverload
namespace Smart.Data.Mapper;

using System.Data;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Mocks;

#pragma warning disable xUnit1051
public sealed class SqlMapperExecuteScalarTest
{
    //--------------------------------------------------------------------------------
    // Execute
    //--------------------------------------------------------------------------------

    [Fact]

    public void ExecuteScalarByObjectParameter()
    {
        using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        con.Open();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

        var count = con.ExecuteScalar<long>("SELECT COUNT(*) FROM Data WHERE Id = @Id", new { Id = 1 });

        Assert.Equal(1L, count);
    }

    [Fact]

    public async Task ExecuteScalarByObjectParameterAsync()
    {
        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        await con.OpenAsync();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

        var count = await con.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM Data WHERE Id = @Id", new { Id = 1 });

        Assert.Equal(1L, count);
    }

    [Fact]

    public void ResultIsNull()
    {
        using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        con.Open();

        var value = con.ExecuteScalar<long>("SELECT NULL");

        Assert.Equal(default, value);
    }

    [Fact]

    public async Task ResultIsNullAsync()
    {
        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        await con.OpenAsync();

        var value = await con.ExecuteScalarAsync<long>("SELECT NULL");

        Assert.Equal(default, value);
    }

    [Fact]

    public void ResultIsConverted()
    {
        using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        con.Open();

        var value = con.ExecuteScalar<string>("SELECT 0");

        Assert.Equal("0", value);
    }

    [Fact]

    public async Task ResultIsConvertedAsync()
    {
        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        await con.OpenAsync();

        var value = await con.ExecuteScalarAsync<string>("SELECT 0");

        Assert.Equal("0", value);
    }

    //--------------------------------------------------------------------------------
    // Cancel
    //--------------------------------------------------------------------------------

    [Fact]

    public async Task ExecuteScalarCancelAsync()
    {
        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        await con.OpenAsync();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

        var cancel = new CancellationToken(true);
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await con.ExecuteScalarAsync<long>(
                    "SELECT COUNT(*) FROM Data WHERE Id = @Id",
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
        using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        var value = con.ExecuteScalar<long>("SELECT 1");

        Assert.Equal(1L, value);
        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task WithoutOpenAsync()
    {
        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        var value = await con.ExecuteScalarAsync<long>("SELECT 1");

        Assert.Equal(1L, value);
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

        using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        con.ExecuteScalar<long>(config, "SELECT 1", new object());

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

        using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        con.ExecuteScalar<long>(config, "SELECT 1");

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

        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        await con.ExecuteScalarAsync<long>(config, "SELECT 1", new object());

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

        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        await con.ExecuteScalarAsync<long>(config, "SELECT 1");

        Assert.False(factory.BuildCalled);
        Assert.False(factory.PostProcessCalled);
    }
}
