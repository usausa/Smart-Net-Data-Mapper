namespace Smart.Data.Mapper
{
    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper.Attributes;

    using Xunit;

    public class AttributeResultTest
    {
        [Fact]

        public void MapByAttribute()
        {
            using var con = new SqliteConnection("Data Source=:memory:");
            con.Open();
            con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text)");
            con.Execute("INSERT INTO Data (Id, Name) VALUES (@Id, @Name)", new { Id = 1, Name = "test" });

            var entity = con.QueryFirstOrDefault<DataEntity>("SELECT * FROM Data WHERE Id = @Id", new { Id = 1 });

            Assert.Equal(1L, entity!.No);
            Assert.Equal("test", entity.Text);
        }

        protected class DataEntity
        {
            [Name("Id")]
            public long No { get; set; }

            [Name("Name")]
            public string? Text { get; set; }
        }
    }
}
