namespace Smart.Data.Mapper
{
    using System;
    using System.Collections.Generic;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper.Mocks;

    using Xunit;

    public class DictionaryParameterTest
    {
        [Fact]

        public void ParameterByDictionary()
        {
            SqlMapperConfig.Default.ConfigureTypeHandlers(config =>
            {
                config[typeof(DateTime)] = new DateTimeTypeHandler();
            });

            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text, Date int)");

                var parameter = new Dictionary<string, object>
                {
                    { "Id", 1 },
                    { "Name", null },
                    { "Date", new DateTime(2000, 1, 1) },
                };
                con.Execute("INSERT INTO Data (Id, Name, Date) VALUES (@Id, @Name, @Date)", parameter);

                var entity = con.QueryFirstOrDefault<Data>("SELECT * FROM Data WHERE Id = @Id", new { Id = 1 });

                Assert.Equal(1L, entity.Id);
                Assert.Null(entity.Name);
                Assert.Equal(new DateTime(2000, 1, 1), entity.Date);
            }
        }

        protected class Data
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public DateTime Date { get; set; }
        }
    }
}
