namespace Smart.Data.Mapper
{
    using System;
    using System.Data;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper.Mocks;

    using Xunit;

    public class SqlMapperExecuteReaderTest
    {
        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        [Fact]

        public void ExecuteReader()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

                using (var reader = con.ExecuteReader("SELECT * FROM Data ORDER BY Id"))
                {
                    Assert.True(reader.Read());
                    Assert.Equal(1, reader.GetInt64(0));
                    Assert.Equal("test1", reader.GetString(1));

                    Assert.True(reader.Read());
                    Assert.Equal(2, reader.GetInt64(0));
                    Assert.Equal("test2", reader.GetString(1));

                    Assert.False(reader.Read());
                }
            }
        }

        [Fact]

        public async Task ExecuteReaderAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

                using (var reader = await con.ExecuteReaderAsync("SELECT * FROM Data ORDER BY Id").ConfigureAwait(false))
                {
                    Assert.True(reader.Read());
                    Assert.Equal(1, reader.GetInt64(0));
                    Assert.Equal("test1", reader.GetString(1));

                    Assert.True(reader.Read());
                    Assert.Equal(2, reader.GetInt64(0));
                    Assert.Equal("test2", reader.GetString(1));

                    Assert.False(reader.Read());
                }
            }
        }

        //--------------------------------------------------------------------------------
        // Cancel
        //--------------------------------------------------------------------------------

        [Fact]

        public async Task ExecuteReaderCancelAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

                await Assert.ThrowsAsync<OperationCanceledException>(async () =>
                {
                    var cancel = new CancellationToken(true);
                    using (await con.ExecuteReaderAsync("SELECT * FROM Data ORDER BY Id", cancel: cancel).ConfigureAwait(false))
                    {
                    }
                }).ConfigureAwait(false);
            }
        }

        //--------------------------------------------------------------------------------
        // Open
        //--------------------------------------------------------------------------------

        [Fact]

        public void WithoutOpen()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                using (var reader = con.ExecuteReader("SELECT 1, 'test1'"))
                {
                    Assert.Equal(ConnectionState.Open, con.State);
                    Assert.True(reader.Read());
                    Assert.False(reader.Read());
                }

                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        [Fact]

        public async Task WithoutOpenAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                using (var reader = await con.ExecuteReaderAsync("SELECT 1, 'test1'").ConfigureAwait(false))
                {
                    Assert.Equal(ConnectionState.Open, con.State);
                    Assert.True(reader.Read());
                    Assert.False(reader.Read());
                }

                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        //--------------------------------------------------------------------------------
        // Close
        //--------------------------------------------------------------------------------

        [Fact]

        public void ClosedConnectionMustClosedWhenQueryError()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                Assert.Throws<SqliteException>(() =>
                {
                    using (con.ExecuteReader("x"))
                    {
                    }
                });

                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        [Fact]

        public void ClosedConnectionMustClosedWhenWhenCommandError()
        {
            using (var con = new CommandUnsupportedConnection())
            {
                Assert.Throws<NotSupportedException>(() =>
                {
                    using (con.ExecuteReader("x"))
                    {
                    }
                });

                Assert.Equal(ConnectionState.Closed, con.State);
            }
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

            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                Assert.Throws<NotSupportedException>(() =>
                {
                    using (con.ExecuteReader(config, "SELECT 1, 'test1'", new object()))
                    {
                    }
                });

                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        [Fact]

        public async Task ClosedConnectionMustClosedWhenQueryErrorAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                await Assert.ThrowsAsync<SqliteException>(async () =>
                {
                    using (await con.ExecuteReaderAsync("x").ConfigureAwait(false))
                    {
                    }
                }).ConfigureAwait(false);

                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        [Fact]

        public async Task ClosedConnectionMustClosedWhenWhenCommandErrorAsync()
        {
            using (var con = new CommandUnsupportedConnection())
            {
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                {
                    using (await con.ExecuteReaderAsync("x").ConfigureAwait(false))
                    {
                    }
                }).ConfigureAwait(false);

                Assert.Equal(ConnectionState.Closed, con.State);
            }
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

            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                await Assert.ThrowsAsync<NotSupportedException>(async () =>
                {
                    using (await con.ExecuteReaderAsync(config, "SELECT 1, 'test1'", new object()).ConfigureAwait(false))
                    {
                    }
                }).ConfigureAwait(false);

                Assert.Equal(ConnectionState.Closed, con.State);
            }
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

            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                using (con.ExecuteReader(config, "SELECT 1, 'test1'", new object()))
                {
                }

                Assert.True(factory.BuildCalled);
                Assert.True(factory.PostProcessCalled);
            }
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

            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                using (con.ExecuteReader(config, "SELECT 1, 'test1'"))
                {
                }

                Assert.False(factory.BuildCalled);
                Assert.False(factory.PostProcessCalled);
            }
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

            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                using (await con.ExecuteReaderAsync(config, "SELECT 1, 'test1'", new object()).ConfigureAwait(false))
                {
                }

                Assert.True(factory.BuildCalled);
                Assert.True(factory.PostProcessCalled);
            }
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

            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                using (await con.ExecuteReaderAsync(config, "SELECT 1, 'test1'").ConfigureAwait(false))
                {
                }

                Assert.False(factory.BuildCalled);
                Assert.False(factory.PostProcessCalled);
            }
        }
    }
}
