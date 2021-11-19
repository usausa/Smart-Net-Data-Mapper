namespace Smart.Data.Mapper;

using System.Collections.Generic;

using Smart.Mock.Data;

using Xunit;

[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Ignore")]
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

        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns, new List<object[]>()));

        Assert.Equal(1, config.Diagnostics.ResultMapperCacheCount);

        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns, new List<object[]>()));

        Assert.Equal(1, config.Diagnostics.ResultMapperCacheCount);
    }

    [Fact]
    public void ResultMapperForSameTypeDifferentResult()
    {
        var config = new SqlMapperConfig();

        var columns1 = new[]
        {
            new MockColumn(typeof(long), "Id")
        };
        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns1, new List<object[]>()));

        Assert.Equal(1, config.Diagnostics.ResultMapperCacheCount);

        var columns2 = new[]
        {
            new MockColumn(typeof(string), "Name")
        };
        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns2, new List<object[]>()));

        Assert.Equal(2, config.Diagnostics.ResultMapperCacheCount);

        var columns3 = new[]
        {
            new MockColumn(typeof(long), "Id"),
            new MockColumn(typeof(string), "Name")
        };
        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns3, new List<object[]>()));

        Assert.Equal(3, config.Diagnostics.ResultMapperCacheCount);
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

        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns, new List<object[]>()));

        Assert.Equal(1, config.Diagnostics.ResultMapperCacheCount);

        ((ISqlMapperConfig)config).CreateResultMapper<Data2Entity>(new MockDataReader(columns, new List<object[]>()));

        Assert.Equal(2, config.Diagnostics.ResultMapperCacheCount);
    }

    protected class DataEntity
    {
        public long Id { get; set; }

        public string? Name { get; set; }
    }

    protected class Data2Entity
    {
        public long Id { get; set; }

        public string? Name { get; set; }
    }
}
