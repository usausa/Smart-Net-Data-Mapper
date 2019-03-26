namespace Smart.Data.Mapper.Mocks
{
    using System;
    using Smart.Data.Mapper.Parameters;

    public sealed class PostProcessErrorParameterBuilderFactory : IParameterBuilderFactory
    {
        public bool IsMatch(Type type) => true;

        public ParameterBuilder CreateBuilder(ISqlMapperConfig config, Type type)
        {
            return new ParameterBuilder(null, (c, v) => throw new NotSupportedException());
        }
    }
}
