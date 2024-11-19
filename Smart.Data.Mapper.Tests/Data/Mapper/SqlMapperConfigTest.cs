namespace Smart.Data.Mapper;

using System.Data;

using Smart.Converter;
using Smart.Data.Mapper.Mocks;
using Smart.Data.Mapper.Selector;
using Smart.Mock.Data;
using Smart.Reflection;

public sealed class SqlMapperConfigTest
{
    [Fact]
    public void CountParameterBuilderCache()
    {
        var config = new SqlMapperConfig();
        ((ISqlMapperConfig)config).CreateParameterBuilder(typeof(object));

        Assert.Equal(1, config.Diagnostics.ParameterBuilderCacheCount);

        config.ClearParameterBuilderCache();

        Assert.Equal(0, config.Diagnostics.ParameterBuilderCacheCount);
    }

    [Fact]
    public void CountResultMapperCache()
    {
        var config = new SqlMapperConfig();
#pragma warning disable CA2000
        ((ISqlMapperConfig)config).CreateResultMapper<object>(new MockDataReader([new MockColumn(typeof(int), "Id")], new List<object[]>()));
#pragma warning restore CA2000

        Assert.Equal(1, config.Diagnostics.ResultMapperCacheCount);

        config.ClearResultMapperCache();

        Assert.Equal(0, config.Diagnostics.ResultMapperCacheCount);
    }

    [Fact]
    public void CountTypeHandleEntriesCache()
    {
        var config = new SqlMapperConfig();
        ((ISqlMapperConfig)config).LookupTypeHandle(typeof(int));

        Assert.Equal(1, config.Diagnostics.TypeHandlerCacheCount);

        config.ClearTypeHandleEntriesCache();

        Assert.Equal(0, config.Diagnostics.TypeHandlerCacheCount);
    }

    [Fact]
    public void CreateParserFailed()
    {
        var config = new SqlMapperConfig();
        config.ConfigureTypeHandlers(static opt => opt.Clear());
        config.ConfigureTypeMap(static opt => opt.Clear());
        config.Converter = new DummyObjectConverter();

        Assert.Null(((ISqlMapperConfig)config).CreateParser(typeof(int), typeof(long)));
    }

    [Fact]
    public void LookupTypeHandleFailed()
    {
        var config = new SqlMapperConfig();
        config.ConfigureTypeHandlers(static opt => opt.Clear());
        config.ConfigureTypeMap(static opt => opt.Clear());

        Assert.Throws<SqlMapperException>(() => ((ISqlMapperConfig)config).LookupTypeHandle(typeof(ParameterDirection)));
    }

    [Fact]
    public void CreateMapperFailed()
    {
        var config = new SqlMapperConfig();
        config.ConfigureResultMapperFactories(static opt => opt.Clear());

#pragma warning disable CA2000
        var reader = new MockDataReader([new MockColumn(typeof(int), "Id")], new List<object[]>());
#pragma warning restore CA2000
        Assert.Throws<SqlMapperException>(() => ((ISqlMapperConfig)config).CreateResultMapper<object>(reader));
    }

    [Fact]
    public void CreateParameterBuilderFailed()
    {
        var config = new SqlMapperConfig();
        config.ConfigureParameterBuilderFactories(static opt => opt.Clear());

        Assert.Throws<SqlMapperException>(() => ((ISqlMapperConfig)config).CreateParameterBuilder(typeof(object)));
    }

    [Fact]
    public void CoverageFix()
    {
        var config = new SqlMapperConfig();

        config.ResetParameterBuilderFactories();
        config.ConfigureParameterBuilderFactories(_ => { });

        config.ResetResultMapperFactories();
        config.ConfigureResultMapperFactories(_ => { });

        config.ResetTypeMap();
        config.ConfigureTypeMap(_ => { });

        config.ResetTypeHandlers();
        config.ConfigureTypeHandlers(_ => { });

        config.DelegateFactory = DelegateFactory.Default;
        Assert.Equal(DelegateFactory.Default, config.DelegateFactory);

        config.Converter = ObjectConverter.Default;
        Assert.Equal(ObjectConverter.Default, config.Converter);

        config.PropertySelector = DefaultPropertySelector.Instance;
        Assert.Equal(DefaultPropertySelector.Instance, config.PropertySelector);
    }
}
