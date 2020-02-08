namespace Smart.Data.Mapper
{
    using System.Collections.Generic;
    using System.Data;

    using Smart.Converter;
    using Smart.Data.Mapper.Mocks;
    using Smart.Data.Mapper.Selector;
    using Smart.Mock.Data;
    using Smart.Reflection;

    using Xunit;

    public class SqlMapperConfigTest
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
            ((ISqlMapperConfig)config).CreateResultMapper<object>(new MockDataReader(new[] { new MockColumn(typeof(int), "Id") }, new List<object[]>()));

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
            config.ConfigureTypeHandlers(opt => opt.Clear());
            config.ConfigureTypeMap(opt => opt.Clear());
            config.Converter = new DummyObjectConverter();

            Assert.Null(((ISqlMapperConfig)config).CreateParser(typeof(int), typeof(long)));
        }

        [Fact]
        public void LookupTypeHandleFailed()
        {
            var config = new SqlMapperConfig();
            config.ConfigureTypeHandlers(opt => opt.Clear());
            config.ConfigureTypeMap(opt => opt.Clear());

            Assert.Throws<SqlMapperException>(() => ((ISqlMapperConfig)config).LookupTypeHandle(typeof(ParameterDirection)));
        }

        [Fact]
        public void CreateMapperFailed()
        {
            var config = new SqlMapperConfig();
            config.ConfigureResultMapperFactories(opt => opt.Clear());

            var reader = new MockDataReader(new[] { new MockColumn(typeof(int), "Id") }, new List<object[]>());
            Assert.Throws<SqlMapperException>(() => ((ISqlMapperConfig)config).CreateResultMapper<object>(reader));
        }

        [Fact]
        public void CreateParameterBuilderFailed()
        {
            var config = new SqlMapperConfig();
            config.ConfigureParameterBuilderFactories(opt => opt.Clear());

            Assert.Throws<SqlMapperException>(() => ((ISqlMapperConfig)config).CreateParameterBuilder(typeof(object)));
        }

        [Fact]
        public void CoverageFix()
        {
            var config = new SqlMapperConfig();

            config.ResetParameterBuilderFactories();
            config.ConfigureParameterBuilderFactories(opt => { });

            config.ResetResultMapperFactories();
            config.ConfigureResultMapperFactories(opt => { });

            config.ResetTypeMap();
            config.ConfigureTypeMap(opt => { });

            config.ResetTypeHandlers();
            config.ConfigureTypeHandlers(opt => { });

            config.DelegateFactory = DelegateFactory.Default;
            Assert.Equal(DelegateFactory.Default, config.DelegateFactory);

            config.Converter = ObjectConverter.Default;
            Assert.Equal(ObjectConverter.Default, config.Converter);

            config.PropertySelector = DefaultPropertySelector.Instance;
            Assert.Equal(DefaultPropertySelector.Instance, config.PropertySelector);
        }
    }
}
