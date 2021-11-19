namespace Smart.Data.Mapper;

using System.Data;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Mocks;

using Xunit;

[System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MethodHasAsyncOverload", Justification = "Ignore")]
public class SqlMapperExecuteScalarTest
{
    //--------------------------------------------------------------------------------
    // Execute
    //--------------------------------------------------------------------------------

    [Fact]

    public void ExecuteScalarByObjectParameter()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

        var count = con.ExecuteScalar<long>("SELECT COUNT(*) FROM Data WHERE Id = @Id", new { Id = 1 });

        Assert.Equal(1L, count);
    }

    [Fact]

    public async ValueTask ExecuteScalarByObjectParameterAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync().ConfigureAwait(false);
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

        var count = await con.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM Data WHERE Id = @Id", new { Id = 1 }).ConfigureAwait(false);

        Assert.Equal(1L, count);
    }

    [Fact]

    public void ResultIsNull()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();

        var value = con.ExecuteScalar<long>("SELECT NULL");

        Assert.Equal(default, value);
    }

    [Fact]

    public async ValueTask ResultIsNullAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync().ConfigureAwait(false);

        var value = await con.ExecuteScalarAsync<long>("SELECT NULL").ConfigureAwait(false);

        Assert.Equal(default, value);
    }

    [Fact]

    public void ResultIsConverted()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();

        var value = con.ExecuteScalar<string>("SELECT 0");

        Assert.Equal("0", value);
    }

    [Fact]

    public async ValueTask ResultIsConvertedAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync().ConfigureAwait(false);

        var value = await con.ExecuteScalarAsync<string>("SELECT 0").ConfigureAwait(false);

        Assert.Equal("0", value);
    }

    //--------------------------------------------------------------------------------
    // Cancel
    //--------------------------------------------------------------------------------

    [Fact]

    public async ValueTask ExecuteScalarCancelAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync().ConfigureAwait(false);
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

        var cancel = new CancellationToken(true);
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await con.ExecuteScalarAsync<long>(
                    "SELECT COUNT(*) FROM Data WHERE Id = @Id",
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
        var value = con.ExecuteScalar<long>("SELECT 1");

        Assert.Equal(1L, value);
        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async ValueTask WithoutOpenAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        var value = await con.ExecuteScalarAsync<long>("SELECT 1").ConfigureAwait(false);

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

        using var con = new SqliteConnection("Data Source=:memory:");
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

        using var con = new SqliteConnection("Data Source=:memory:");
        con.ExecuteScalar<long>(config, "SELECT 1");

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
        await con.ExecuteScalarAsync<long>(config, "SELECT 1", new object()).ConfigureAwait(false);

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
        await con.ExecuteScalarAsync<long>(config, "SELECT 1").ConfigureAwait(false);

        Assert.False(factory.BuildCalled);
        Assert.False(factory.PostProcessCalled);
    }
}
