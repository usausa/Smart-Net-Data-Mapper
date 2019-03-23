namespace Smart.Data.Mapper
{
    using System.Linq;

    using Microsoft.Data.Sqlite;

    using Xunit;

    public class SqlMapperQueryTest
    {
        [Fact]

        public void Query()
        {
            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
                con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

                var list = con.Query<Data>("SELECT * FROM Data ORDER BY Id").ToList();

                Assert.Equal(2, list.Count);
                Assert.Equal(1, list[0].Id);
                Assert.Equal("test1", list[0].Name);
                Assert.Equal(2, list[1].Id);
                Assert.Equal("test2", list[1].Name);
            }
        }

        // TODO
        //[Fact]

        //public async Task QueryAsync()
        //{
        //    using (var con = new SqliteConnection("Data Source=:memory:"))
        //    {
        //        con.Open();
        //        con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
        //        con.Execute("INSERT INTO Data (Id, Name) VALUES (1, 'test1')");
        //        con.Execute("INSERT INTO Data (Id, Name) VALUES (2, 'test2')");

        //        var list = (await con.QueryAsync<Data>("SELECT * FROM Data ORDER BY Id")).ToList();

        //        Assert.Equal(2, list.Count);
        //        Assert.Equal(1, list[0].Id);
        //        Assert.Equal("test1", list[0].Name);
        //        Assert.Equal(2, list[1].Id);
        //        Assert.Equal("test2", list[1].Name);
        //    }
        //}

        protected class Data
        {
            public int Id { get; set; }

            public string Name { get; set; }
        }
    }
}
