namespace Smart.Data.Mapper
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper.Mocks;

    using Xunit;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("ReSharper", "MethodHasAsyncOverload", Justification = "Ignore")]
    public class SqlMapperQueryListTest
    {
        //--------------------------------------------------------------------------------
        // QueryList
        //--------------------------------------------------------------------------------

        [Fact]

        public void QueryList()
        {
            using var con = new SqliteConnection("Data Source=:memory:");
            con.Open();
            con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
            con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
            con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

            var list = con.QueryList<DataEntity>("SELECT * FROM Data ORDER BY Id");

            Assert.Equal(2, list.Count);
            Assert.Equal(1, list[0].Id);
            Assert.Equal("test1", list[0].Name);
            Assert.Equal(2, list[1].Id);
            Assert.Equal("test2", list[1].Name);
        }

        [Fact]

        public async ValueTask QueryListAsync()
        {
            await using var con = new SqliteConnection("Data Source=:memory:");
            con.Open();
            con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
            con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
            con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

            var list = await con.QueryListAsync<DataEntity>("SELECT * FROM Data ORDER BY Id").ConfigureAwait(false);

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

        public async ValueTask QueryListCancelAsync()
        {
            await using var con = new SqliteConnection("Data Source=:memory:");
            con.Open();
            con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

            await Assert.ThrowsAsync<OperationCanceledException>(async () =>
            {
                var cancel = new CancellationToken(true);
                await con.QueryListAsync<DataEntity>(
                        "SELECT * FROM Data ORDER BY Id",
                        cancel: cancel)
                    .ConfigureAwait(false);
            }).ConfigureAwait(false);
        }

        //--------------------------------------------------------------------------------
        // Open
        //--------------------------------------------------------------------------------

        [Fact]

        public void WithoutOpen()
        {
            using var con = new SqliteConnection("Data Source=:memory:");
            con.QueryList<DataEntity>("SELECT 1, 'test1'");

            Assert.Equal(ConnectionState.Closed, con.State);
        }

        [Fact]

        public async ValueTask WithoutOpenAsync()
        {
            await using var con = new SqliteConnection("Data Source=:memory:");
            await con.QueryListAsync<DataEntity>("SELECT 1, 'test1'").ConfigureAwait(false);

            Assert.Equal(ConnectionState.Closed, con.State);
        }

        //--------------------------------------------------------------------------------
        // Close
        //--------------------------------------------------------------------------------

        [Fact]

        public void ClosedConnectionMustClosedWhenQueryListError()
        {
            using var con = new SqliteConnection("Data Source=:memory:");
            Assert.Throws<SqliteException>(() => con.QueryList<DataEntity>("x"));

            Assert.Equal(ConnectionState.Closed, con.State);
        }

        [Fact]

        public void ClosedConnectionMustClosedWhenCreateCommandError()
        {
            using var con = new CommandUnsupportedConnection();
            Assert.Throws<NotSupportedException>(() => con.QueryList<DataEntity>("x"));

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
            Assert.Throws<NotSupportedException>(() => con.QueryList<DataEntity>(config, "SELECT 1, 'test1'", new object()));

            Assert.Equal(ConnectionState.Closed, con.State);
        }

        [Fact]

        public async ValueTask ClosedConnectionMustClosedWhenQueryListErrorAsync()
        {
            await using var con = new SqliteConnection("Data Source=:memory:");
            await Assert.ThrowsAsync<SqliteException>(async () => await con.QueryListAsync<DataEntity>("x").ConfigureAwait(false)).ConfigureAwait(false);

            Assert.Equal(ConnectionState.Closed, con.State);
        }

        [Fact]

        public async ValueTask ClosedConnectionMustClosedWhenCreateCommandErrorAsync()
        {
            await using var con = new CommandUnsupportedConnection();
            await Assert.ThrowsAsync<NotSupportedException>(async () => await con.QueryListAsync<DataEntity>("x").ConfigureAwait(false)).ConfigureAwait(false);

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

            await using var con = new SqliteConnection("Data Source=:memory:");
            await Assert.ThrowsAsync<NotSupportedException>(async () => await con.QueryListAsync<DataEntity>(config, "SELECT 1, 'test1'", new object()).ConfigureAwait(false)).ConfigureAwait(false);

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
            var list = con.QueryList<DataEntity>(config, "SELECT 1, 'test1'", new object());

            Assert.Single(list);
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
            var list = con.QueryList<DataEntity>(config, "SELECT 1, 'test1'");

            Assert.Single(list);
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

            await using var con = new SqliteConnection("Data Source=:memory:");
            var list = await con.QueryListAsync<DataEntity>(config, "SELECT 1, 'test1'", new object()).ConfigureAwait(false);

            Assert.Single(list);
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

            await using var con = new SqliteConnection("Data Source=:memory:");
            var list = await con.QueryListAsync<DataEntity>(config, "SELECT 1, 'test1'").ConfigureAwait(false);

            Assert.Single(list);
            Assert.False(factory.BuildCalled);
            Assert.False(factory.PostProcessCalled);
        }

        protected class DataEntity
        {
            public int Id { get; set; }

            public string? Name { get; set; }
        }
    }
}
