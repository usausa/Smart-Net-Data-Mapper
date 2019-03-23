namespace Smart.Data.Mapper
{
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Xunit;

    public class SqlMapperExecuteScalarTest
    {
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

                Assert.Equal(1, count);
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

                Assert.Equal(1, count);
            }
        }
    }
}
