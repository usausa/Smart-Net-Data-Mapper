namespace Smart.Data.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Smart.Data.Mapper.Attributes;
    using Smart.Mock.Data;

    using Xunit;

    public class ObjectResultMapperFactoryTest
    {
        [Fact]
        public void MapProperty()
        {
            using (var con = new MockDbConnection())
            {
                var columns = new[]
                {
                    new MockColumn(typeof(int), "Column1"),
                    new MockColumn(typeof(int), "Column2"),
                    new MockColumn(typeof(int), "Column3"),
                    new MockColumn(typeof(int), "Column4"),
                    new MockColumn(typeof(int), "Column5"),
                    new MockColumn(typeof(int), "Column6"),
                };
                var values = new List<object[]>
                {
                    new object[] { 1, 1, 1, 1, 1, 1 },
                    new object[] { DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value }
                };

                var reader = new MockDataReader(columns, values);
                con.SetupCommand(cmd => cmd.SetupResult(reader));

                var list = con.Query<Data>("SELECT * FROM Data").ToList();

                Assert.Equal(2, list.Count);
                Assert.Equal(1, list[0].Column1);
                Assert.Equal(1, list[0].Column2);
                Assert.Equal(1, list[0].Column3);
                Assert.Equal(0, list[0].Column4);
                Assert.Equal(0, list[0].Column5);
                Assert.Equal(0, list[1].Column1);
                Assert.Null(list[1].Column2);
                Assert.Equal(0, list[1].Column3);
                Assert.Equal(0, list[1].Column4);
                Assert.Equal(0, list[1].Column5);
            }
        }

        protected class Data
        {
            public int Column1 { get; set; }

            public int? Column2 { get; set; }

            public long Column3 { get; set; }

            public int Column4 => 0;

            [Ignore]
            public int Column5 { get; set; }
        }
    }
}
