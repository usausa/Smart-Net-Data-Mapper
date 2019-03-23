namespace Smart.Data.Mapper
{
    using System;
    using System.Data;

    using Microsoft.Data.Sqlite;

    using Smart.Data.Mapper.Handlers;

    using Xunit;

    public class DynamicParameterTest
    {
        [Fact]

        public void ParameterByDynamicParameter()
        {
            SqlMapperConfig.Default.ConfigureTypeHandlers(config =>
            {
                config[typeof(DateTime)] = new DateTimeTypeHandler();
            });

            using (var con = new SqliteConnection("Data Source=:memory:"))
            {
                con.Open();
                con.Execute("CREATE TABLE IF NOT EXISTS Data (Id int PRIMARY KEY, Name text, Date int)");

                var parameter = new DynamicParameter();
                parameter.Add("Id", 1);
                parameter.Add("Name", null, DbType.StringFixedLength, 10);
                parameter.Add("Date", new DateTime(2000, 1, 1));
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

        protected class DateTimeTypeHandler : TypeHandler<DateTime>
        {
            public override void SetValue(IDbDataParameter parameter, DateTime value)
            {
                parameter.DbType = DbType.Int64;
                parameter.Value = value.Ticks;
            }

            public override DateTime Parse(object value)
            {
                return new DateTime((long)value);
            }
        }
    }
}
