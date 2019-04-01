namespace Smart.Data.Mapper
{
    using System.Data;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper.Mocks;

    using Xunit;

    public class SqlMapperExecuteTest
    {
        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        [Fact]

        public void ExecuteByObjectParameter()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

                var effect = con.Execute("INSERT INTO Data (Id, Name) VALUES (@Id, @Name)", new { Id = 1, Name = "test" });

                Assert.Equal(1, effect);
            }
        }

        [Fact]

        public async Task ExecuteByObjectParameterAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

                var effect = await con.ExecuteAsync("INSERT INTO Data (Id, Name) VALUES (@Id, @Name)", new { Id = 1, Name = "test" }).ConfigureAwait(false);

                Assert.Equal(1, effect);
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
                con.Execute("PRAGMA AUTO_VACUUM=1");

                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        [Fact]

        public async Task WithoutOpenAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                await con.ExecuteAsync("PRAGMA AUTO_VACUUM=1").ConfigureAwait(false);

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
                con.Execute(config, "PRAGMA AUTO_VACUUM=1", new object());

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
                con.Execute(config, "PRAGMA AUTO_VACUUM=1");

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
                await con.ExecuteAsync(config, "PRAGMA AUTO_VACUUM=1", new object()).ConfigureAwait(false);

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
                await con.ExecuteAsync(config, "PRAGMA AUTO_VACUUM=1").ConfigureAwait(false);

                Assert.False(factory.BuildCalled);
                Assert.False(factory.PostProcessCalled);
            }
        }
    }
}
