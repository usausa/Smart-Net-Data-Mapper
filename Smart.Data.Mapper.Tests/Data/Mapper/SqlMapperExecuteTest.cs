namespace Smart.Data.Mapper;

using System.Data;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Mocks;

using Xunit;

[System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MethodHasAsyncOverload", Justification = "Ignore")]
public class SqlMapperExecuteTest
{
    //--------------------------------------------------------------------------------
    // Execute
    //--------------------------------------------------------------------------------

    [Fact]

    public void ExecuteByObjectParameter()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

        var effect = con.Execute("INSERT INTO Data (Id, Name) VALUES (@Id, @Name)", new { Id = 1, Name = "test" });

        Assert.Equal(1, effect);
    }

    [Fact]

    public async Task ExecuteByObjectParameterAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

        var effect = await con.ExecuteAsync("INSERT INTO Data (Id, Name) VALUES (@Id, @Name)", new { Id = 1, Name = "test" });

        Assert.Equal(1, effect);
    }

    //--------------------------------------------------------------------------------
    // Cancel
    //--------------------------------------------------------------------------------

    [Fact]

    public async Task ExecuteCancelAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

        var cancel = new CancellationToken(true);
        await Assert.ThrowsAsync<TaskCanceledException>(async () =>
                await con.ExecuteAsync(
                    "INSERT INTO Data (Id, Name) VALUES (@Id, @Name)",
                    new { Id = 1, Name = "test" },
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
        con.Execute("PRAGMA AUTO_VACUUM=1");

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task WithoutOpenAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.ExecuteAsync("PRAGMA AUTO_VACUUM=1");

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
        con.Execute(config, "PRAGMA AUTO_VACUUM=1", new object());

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
        con.Execute(config, "PRAGMA AUTO_VACUUM=1");

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
        await con.ExecuteAsync(config, "PRAGMA AUTO_VACUUM=1", new object());

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
        await con.ExecuteAsync(config, "PRAGMA AUTO_VACUUM=1");

        Assert.False(factory.BuildCalled);
        Assert.False(factory.PostProcessCalled);
    }
}
