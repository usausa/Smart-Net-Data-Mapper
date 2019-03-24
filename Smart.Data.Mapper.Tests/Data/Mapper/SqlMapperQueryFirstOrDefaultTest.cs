namespace Smart.Data.Mapper
{
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Xunit;

    public class SqlMapperQueryFirstOrDefaultTest
    {
        [Fact]

        public void QueryFirstOrDefault()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");

                var entity = con.QueryFirstOrDefault<Data>("SELECT * FROM Data ORDER BY Id");

                Assert.Equal(1, entity.Id);
                Assert.Equal("test1", entity.Name);
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

                var entity = await con.QueryFirstOrDefaultAsync<Data>("SELECT * FROM Data ORDER BY Id");

                Assert.Equal(1, entity.Id);
                Assert.Equal("test1", entity.Name);
            }
        }

        protected class Data
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
