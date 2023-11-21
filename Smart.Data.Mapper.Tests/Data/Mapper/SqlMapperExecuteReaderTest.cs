namespace Smart.Data.Mapper;

using System.Data;

using Microsoft.Data.Sqlite;

using Smart.Data.Mapper.Mocks;

using Xunit;

[System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MethodHasAsyncOverload", Justification = "Ignore")]
[System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
public class SqlMapperExecuteReaderTest
{
    //--------------------------------------------------------------------------------
    // Execute
    //--------------------------------------------------------------------------------

    [Fact]
    public void ExecuteReader()
    {
        using var con = new SqliteConnection("Data Source=:memory:");
        con.Open();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
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
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
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
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");
    }

    [Fact]
    public void ExecuteReaderLife()
    {
        Prepare("ExecuteReaderLife.db");
        using var reader = new SqliteConnection("Data Source=ExecuteReaderLife.db").ExecuteReader("SELECT * FROM Data ORDER BY Id");
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
        using var reader = await new SqliteConnection("Data Source=ExecuteReaderLifeAsync.db").ExecuteReaderAsync("SELECT * FROM Data ORDER BY Id");
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
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        await con.OpenAsync();
        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

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
        using var con = new SqliteConnection("Data Source=:memory:");
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
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
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
        using var con = new SqliteConnection("Data Source=:memory:");
        Assert.Throws<SqliteException>(() =>
        {
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

        using var con = new SqliteConnection("Data Source=:memory:");
        Assert.Throws<NotSupportedException>(() =>
        {
            using (con.ExecuteReader(config, "SELECT 1, 'test1'", new object()))
            {
            }
        });

        Assert.Equal(ConnectionState.Closed, con.State);
    }

    [Fact]

    public async Task ClosedConnectionMustClosedWhenQueryErrorAsync()
    {
#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
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
#pragma warning disable CA2007
        await using var con = new CommandUnsupportedConnection();
#pragma warning restore CA2007
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

#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
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

        using var con = new SqliteConnection("Data Source=:memory:");
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

        using var con = new SqliteConnection("Data Source=:memory:");
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

#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
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

#pragma warning disable CA2007
        await using var con = new SqliteConnection("Data Source=:memory:");
#pragma warning restore CA2007
        using (await con.ExecuteReaderAsync(config, "SELECT 1, 'test1'"))
        {
        }

        Assert.False(factory.BuildCalled);
        Assert.False(factory.PostProcessCalled);
    }
}
