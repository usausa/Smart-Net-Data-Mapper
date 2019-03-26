namespace Smart.Data.Mapper
{
    using System.Data;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper.Mocks;

    using Xunit;

    public class SqlMapperQueryFirstOrDefaultTest
    {
        //--------------------------------------------------------------------------------
        // Query
        //--------------------------------------------------------------------------------

        [Fact]

        public void QueryFirstOrDefault()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                var entity = con.QueryFirstOrDefault<Data>("SELECT * FROM Data WHERE Id = @Id", new { Id = 1 });

                Assert.NotNull(entity);
                Assert.Equal(1, entity.Id);
                Assert.Equal("test1", entity.Name);

                entity = con.QueryFirstOrDefault<Data>("SELECT * FROM Data WHERE Id = @Id", new { Id = 0 });

                Assert.Null(entity);
            }
        }

        [Fact]

        public async Task QueryFirstOrDefaultAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                var entity = await con.QueryFirstOrDefaultAsync<Data>("SELECT * FROM Data WHERE Id = @Id", new { Id = 1 });

                Assert.NotNull(entity);
                Assert.Equal(1, entity.Id);
                Assert.Equal("test1", entity.Name);

                entity = await con.QueryFirstOrDefaultAsync<Data>("SELECT * FROM Data WHERE Id = @Id", new { Id = 0 });

                Assert.Null(entity);
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
                con.QueryFirstOrDefault<Data>("SELECT 1, 'test1'");

                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        [Fact]

        public async Task WithoutOpenAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                await con.QueryFirstOrDefaultAsync<Data>("SELECT 1, 'test1'");

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
                Assert.Throws<SqliteException>(() => con.QueryFirstOrDefault<Data>("x"));

                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        [Fact]

        public async Task ClosedConnectionMustClosedWhenQueryErrorAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                await Assert.ThrowsAsync<SqliteException>(async () => await con.QueryFirstOrDefaultAsync<Data>("x"));

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
                con.QueryFirstOrDefault<Data>(config, "SELECT 1, 'test1'", new object());

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
                con.QueryFirstOrDefault<Data>(config, "SELECT 1, 'test1'");

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
                await con.QueryFirstOrDefaultAsync<Data>(config, "SELECT 1, 'test1'", new object());

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
                await con.QueryFirstOrDefaultAsync<Data>(config, "SELECT 1, 'test1'");

                Assert.False(factory.BuildCalled);
                Assert.False(factory.PostProcessCalled);
            }
        }

        protected class Data
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
