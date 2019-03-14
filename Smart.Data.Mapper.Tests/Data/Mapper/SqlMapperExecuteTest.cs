namespace Smart.Data.Mapper
{
    using Microsoft.Data.Sqlite;

    using Xunit;

    public class SqlMapperExecuteTest
    {
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
    }
}
