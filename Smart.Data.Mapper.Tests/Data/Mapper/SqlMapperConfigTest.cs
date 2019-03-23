namespace Smart.Data.Mapper
{
    using Smart.Converter;
    using Smart.Data.Mapper.Selector;
    using Smart.Reflection;

    using Xunit;

    public class SqlMapperConfigTest
    {
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
