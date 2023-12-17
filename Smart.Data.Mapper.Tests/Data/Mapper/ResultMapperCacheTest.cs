namespace Smart.Data.Mapper;

using Smart.Mock.Data;

public sealed class ResultMapperCacheTest
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

#pragma warning disable CA2000
        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns, new List<object[]>()));
#pragma warning restore CA2000

        Assert.Equal(1, config.Diagnostics.ResultMapperCacheCount);

#pragma warning disable CA2000
        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns, new List<object[]>()));
#pragma warning restore CA2000

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
#pragma warning disable CA2000
        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns1, new List<object[]>()));
#pragma warning restore CA2000

        Assert.Equal(1, config.Diagnostics.ResultMapperCacheCount);

        var columns2 = new[]
        {
            new MockColumn(typeof(string), "Name")
        };
#pragma warning disable CA2000
        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns2, new List<object[]>()));
#pragma warning restore CA2000

        Assert.Equal(2, config.Diagnostics.ResultMapperCacheCount);

        var columns3 = new[]
        {
            new MockColumn(typeof(long), "Id"),
            new MockColumn(typeof(string), "Name")
        };
#pragma warning disable CA2000
        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns3, new List<object[]>()));
#pragma warning restore CA2000

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

#pragma warning disable CA2000
        ((ISqlMapperConfig)config).CreateResultMapper<DataEntity>(new MockDataReader(columns, new List<object[]>()));
#pragma warning restore CA2000

        Assert.Equal(1, config.Diagnostics.ResultMapperCacheCount);

#pragma warning disable CA2000
        ((ISqlMapperConfig)config).CreateResultMapper<Data2Entity>(new MockDataReader(columns, new List<object[]>()));
#pragma warning restore CA2000

        Assert.Equal(2, config.Diagnostics.ResultMapperCacheCount);
    }

    public sealed class DataEntity
    {
        public long Id { get; set; }

        public string? Name { get; set; }
    }

    public sealed class Data2Entity
    {
        public long Id { get; set; }

        public string? Name { get; set; }
    }
}
