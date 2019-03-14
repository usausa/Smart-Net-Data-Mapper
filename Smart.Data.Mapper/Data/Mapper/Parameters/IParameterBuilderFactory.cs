namespace Smart.Data.Mapper.Parameters
{
    using System;

    public interface IParameterBuilderFactory
    {
        bool IsMatch(Type type);

        ParameterBuilder CreateBuilder(ISqlMapperConfig config, Type type);
    }
}
