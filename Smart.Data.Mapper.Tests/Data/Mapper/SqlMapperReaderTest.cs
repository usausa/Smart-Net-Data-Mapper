namespace Smart.Data.Mapper
{
    using System.Data;
    using System.Threading.Tasks;

    using Microsoft.Data.Sqlite;

    using Xunit;

    public class SqlMapperReaderTest
    {
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

        //[Fact]

        //public async Task WithoutOpenAsync()
        //{
        //    using (var con = new SqliteConnection("Data Source=:memory:"))
        //    {
        //        var list = (await con.QueryAsync<Data>("SELECT 1, 'test1'")).ToList();

        //        Assert.Single(list);
        //        Assert.Equal(ConnectionState.Closed, con.State);
        //    }
        //}

        [Fact]

        public void Reader()
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

        public async Task ReaderAsync()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

                using (var reader = await con.ExecuteReaderAsync("SELECT * FROM Data ORDER BY Id"))
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

        protected class Data
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
