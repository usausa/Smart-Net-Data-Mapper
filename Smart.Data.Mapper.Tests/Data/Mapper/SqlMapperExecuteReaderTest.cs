// ReSharper disable MethodHasAsyncOverload
namespace Smart.Data.Mapper;

using System.Data;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Mocks;

#pragma warning disable xUnit1051
public sealed class SqlMapperExecuteReaderTest
{
    //--------------------------------------------------------------------------------
    // Execute
    //--------------------------------------------------------------------------------

    [Fact]
    public void ExecuteReader()
    {
        using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        con.Open();
        con.Execute("CREATE TABLE Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

        using var reader = con.ExecuteReader("SELECT * FROM Data ORDER BY Id");
        Assert.True(reader.Read());
        Assert.Equal(1, reader.GetInt64(0));
        Assert.Equal("test1", reader.GetString(1));

        Assert.True(reader.Read());
        Assert.Equal(2, reader.GetInt64(0));
        Assert.Equal("test2", reader.GetString(1));

        Assert.False(reader.Read());
    }

    [Fact]
    public async Task ExecuteReaderAsync()
    {
        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        await con.OpenAsync();
        con.Execute("CREATE TABLE Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

        using var reader = await con.ExecuteReaderAsync("SELECT * FROM Data ORDER BY Id");
        Assert.True(reader.Read());
        Assert.Equal(1, reader.GetInt64(0));
        Assert.Equal("test1", reader.GetString(1));

        Assert.True(reader.Read());
        Assert.Equal(2, reader.GetInt64(0));
        Assert.Equal("test2", reader.GetString(1));

        Assert.False(reader.Read());
    }

    //--------------------------------------------------------------------------------
    // Lifecycle
    //--------------------------------------------------------------------------------

    private static void Prepare(string database)
    {
        File.Delete(database);
        using var con = new SqliteConnection($"Data Source={database}");
        con.Open();
        con.Execute("CREATE TABLE Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");
    }

    [Fact]
    public void ExecuteReaderLife()
    {
        Prepare("ExecuteReaderLife.db");
        using var con = new SqliteConnection("Data Source=ExecuteReaderLife.db");
        using var reader = con.ExecuteReader("SELECT * FROM Data ORDER BY Id");
        Assert.True(reader.Read());
        Assert.Equal(1, reader.GetInt64(0));
        Assert.Equal("test1", reader.GetString(1));

        Assert.True(reader.Read());
        Assert.Equal(2, reader.GetInt64(0));
        Assert.Equal("test2", reader.GetString(1));

        Assert.False(reader.Read());
    }

    [Fact]
    public async Task ExecuteReaderLifeAsync()
    {
        Prepare("ExecuteReaderLifeAsync.db");
        await using var con = new SqliteConnection("Data Source=ExecuteReaderLifeAsync.db");
        using var reader = await con.ExecuteReaderAsync("SELECT * FROM Data ORDER BY Id");
        Assert.True(reader.Read());
        Assert.Equal(1, reader.GetInt64(0));
        Assert.Equal("test1", reader.GetString(1));

        Assert.True(reader.Read());
        Assert.Equal(2, reader.GetInt64(0));
        Assert.Equal("test2", reader.GetString(1));

        Assert.False(reader.Read());
    }

    //--------------------------------------------------------------------------------
    // Cancel
    //--------------------------------------------------------------------------------

    [Fact]

    public async Task ExecuteReaderCancelAsync()
    {
        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        await con.OpenAsync();
        con.Execute("CREATE TABLE Data (Id int PRIMARY KEY, Name text)");

        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            var cancel = new CancellationToken(true);
            using (await con.ExecuteReaderAsync("SELECT * FROM Data ORDER BY Id", cancel: cancel))
            {
            }
        });
    }

    //--------------------------------------------------------------------------------
    // Open
    //--------------------------------------------------------------------------------

    [Fact]

    public void WithoutOpen()
    {
        using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        using (var reader = con.ExecuteReader("SELECT 1, 'test1'"))
        {
            Assert.Equal(ConnectionState.Open, con.State);
            Assert.True(reader.Read());
            Assert.False(reader.Read());
        }

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task WithoutOpenAsync()
    {
        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        using (var reader = await con.ExecuteReaderAsync("SELECT 1, 'test1'"))
        {
            Assert.Equal(ConnectionState.Open, con.State);
            Assert.True(reader.Read());
            Assert.False(reader.Read());
        }

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    //--------------------------------------------------------------------------------
    // Close
    //--------------------------------------------------------------------------------

    [Fact]

    public void ClosedConnectionMustClosedWhenQueryError()
    {
        using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        Assert.Throws<SqliteException>(() =>
        {
            // ReSharper disable once AccessToDisposedClosure
            using (con.ExecuteReader("x"))
            {
            }
        });

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public void ClosedConnectionMustClosedWhenWhenCommandError()
    {
        using var con = new CommandUnsupportedConnection();
        Assert.Throws<NotSupportedException>(() =>
        {
            // ReSharper disable once AccessToDisposedClosure
            using (con.ExecuteReader("x"))
            {
            }
        });

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

        using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        Assert.Throws<NotSupportedException>(() =>
        {
            // ReSharper disable once AccessToDisposedClosure
            using (con.ExecuteReader(config, "SELECT 1, 'test1'", new object()))
            {
            }
        });

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task ClosedConnectionMustClosedWhenQueryErrorAsync()
    {
        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        await Assert.ThrowsAsync<SqliteException>(async () =>
        {
            using (await con.ExecuteReaderAsync("x"))
            {
            }
        });

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task ClosedConnectionMustClosedWhenWhenCommandErrorAsync()
    {
        await using var con = new CommandUnsupportedConnection();
        await Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            using (await con.ExecuteReaderAsync("x"))
            {
            }
        });

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

        await using var con = new SqliteConnection($"Data Source=file:{Guid.NewGuid():N}?mode=memory&cache=shared");
        await Assert.ThrowsAsync<NotSupportedException>(async () =>
        {
            using (await con.ExecuteReaderAsync(config, "SELECT 1, 'test1'", new object()))
            {
            }
        });

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
        using (con.ExecuteReader(config, "SELECT 1, 'test1'", new object()))
        {
        }

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
        using (con.ExecuteReader(config, "SELECT 1, 'test1'"))
        {
        }

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
        using (await con.ExecuteReaderAsync(config, "SELECT 1, 'test1'", new object()))
        {
        }

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
        using (await con.ExecuteReaderAsync(config, "SELECT 1, 'test1'"))
        {
        }

        Assert.False(factory.BuildCalled);
        Assert.False(factory.PostProcessCalled);
    }
}
