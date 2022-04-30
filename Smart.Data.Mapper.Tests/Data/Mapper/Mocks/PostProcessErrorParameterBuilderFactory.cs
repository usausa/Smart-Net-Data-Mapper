namespace Smart.Data.Mapper.Mocks;

using Smart.Data.Mapper.Parameters;

public sealed class PostProcessErrorParameterBuilderFactory : IParameterBuilderFactory
{
    public bool IsMatch(Type type) => true;

    public ParameterBuilder CreateBuilder(ISqlMapperConfig config, Type type)
    {
        return new(null, (_, _) => throw new NotSupportedException());
    }
}
