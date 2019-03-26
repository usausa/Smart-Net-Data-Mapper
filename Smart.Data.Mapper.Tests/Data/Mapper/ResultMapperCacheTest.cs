namespace Smart.Data.Mapper
{
    using System.Collections.Generic;

    using Smart.Mock.Data;

    using Xunit;

    public class ResultMapperCacheTest
    {
        [Fact]
        public void ResultMapperCached()
        {
            var config = new SqlMapperConfig();

            var columns = new[]
            {
                new MockColumn(typeof(long), "Id"),
                new MockColumn(typeof(string), "Name")
            };

            ((ISqlMapperConfig)config).CreateResultMapper<Data>(new MockDataReader(columns, new List<object[]>()));

            Assert.Equal(1, config.CountResultMapperCache);

            ((ISqlMapperConfig)config).CreateResultMapper<Data>(new MockDataReader(columns, new List<object[]>()));

            Assert.Equal(1, config.CountResultMapperCache);
        }

        [Fact]
        public void ResultMapperForSameTypeDifferentResult()
        {
            var config = new SqlMapperConfig();

            var columns1 = new[]
            {
                new MockColumn(typeof(long), "Id")
            };
            ((ISqlMapperConfig)config).CreateResultMapper<Data>(new MockDataReader(columns1, new List<object[]>()));

            Assert.Equal(1, config.CountResultMapperCache);

            var columns2 = new[]
            {
                new MockColumn(typeof(string), "Name")
            };
            ((ISqlMapperConfig)config).CreateResultMapper<Data>(new MockDataReader(columns2, new List<object[]>()));

            Assert.Equal(2, config.CountResultMapperCache);

            var columns3 = new[]
            {
                new MockColumn(typeof(long), "Id"),
                new MockColumn(typeof(string), "Name")
            };
            ((ISqlMapperConfig)config).CreateResultMapper<Data>(new MockDataReader(columns3, new List<object[]>()));

            Assert.Equal(3, config.CountResultMapperCache);
        }

        [Fact]
        public void ResultMapperForDifferentTypeSameResult()
        {
            var config = new SqlMapperConfig();

            var columns = new[]
            {
                new MockColumn(typeof(long), "Id"),
                new MockColumn(typeof(string), "Name")
            };

            ((ISqlMapperConfig)config).CreateResultMapper<Data>(new MockDataReader(columns, new List<object[]>()));

            Assert.Equal(1, config.CountResultMapperCache);

            ((ISqlMapperConfig)config).CreateResultMapper<Data2>(new MockDataReader(columns, new List<object[]>()));

            Assert.Equal(2, config.CountResultMapperCache);
        }

        protected class Data
        {
            public long Id { get; set; }

            public string Name { get; set; }
        }

        protected class Data2
        {
            public long Id { get; set; }

            public string Name { get; set; }
        }
    }
}