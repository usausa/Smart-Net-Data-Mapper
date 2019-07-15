namespace Smart.Data.Mapper
{
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper.Mocks;
    using Smart.Mock.Data;

    using Xunit;

    public class SqlMapperTest
    {
        [Fact]

        public void WithTransaction()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");

                using (var tx = con.BeginTransaction())
                {
                    var effect = con.Execute("INSERT INTO Data (Id, Name) VALUES (@Id, @Name)", new { Id = 1, Name = "test" }, tx);

                    Assert.Equal(1, effect);

                    tx.Rollback();
                }

                var count = con.ExecuteScalar<long>("SELECT COUNT(*) FROM Data");

                Assert.Equal(0, count);
            }
        }

        [Fact]

        public void WithTimeout()
        {
            using (var con = new MockDbConnection())
            {
                con.SetupCommand(cmd => cmd.SetupResult(0));

                con.Execute("TEST", commandTimeout: 10);

                Assert.Equal(10, con.Commands[0].CommandTimeout);
            }
        }
    }
}
