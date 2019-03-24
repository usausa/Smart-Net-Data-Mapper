namespace Smart.Data.Mapper
{
    using System.Data;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Xunit;

    public class SqlMapperExecuteScalarTest
    {
        [Fact]

        public void WithoutOpen()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                var value = con.ExecuteScalar<long>("SELECT 1");

                Assert.Equal(1L, value);
                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        [Fact]

        public async Task WithoutOpenAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                var value = await con.ExecuteScalarAsync<long>("SELECT 1");

                Assert.Equal(1L, value);
                Assert.Equal(ConnectionState.Closed, con.State);
            }
        }

        [Fact]

        public void ExecuteScalarByObjectParameter()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

                var count = con.ExecuteScalar<long>("SELECT COUNT(*) FROM Data WHERE Id = @Id", new { Id = 1 });

                Assert.Equal(1L, count);
            }
        }

        [Fact]

        public async Task ExecuteScalarByObjectParameterAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

                var count = await con.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM Data WHERE Id = @Id", new { Id = 1 });

                Assert.Equal(1L, count);
            }
        }

        [Fact]

        public void ResultIsNull()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();

                var value = con.ExecuteScalar<long>("SELECT NULL");

                Assert.Equal(default, value);
            }
        }

        [Fact]

        public async Task ResultIsNullAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();

                var value = await con.ExecuteScalarAsync<long>("SELECT NULL");

                Assert.Equal(default, value);
            }
        }

        [Fact]

        public void ResultIsConverted()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();

                var value = con.ExecuteScalar<string>("SELECT 0");

                Assert.Equal("0", value);
            }
        }

        [Fact]

        public async Task ResultIsConvertedAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();

                var value = await con.ExecuteScalarAsync<string>("SELECT 0");

                Assert.Equal("0", value);
            }
        }
    }
}
