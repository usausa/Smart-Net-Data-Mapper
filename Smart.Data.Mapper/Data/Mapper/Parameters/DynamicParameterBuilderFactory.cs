namespace Smart.Data.Mapper.Parameters;

public sealed class DynamicParameterBuilderFactory : IParameterBuilderFactory
{
    public static DynamicParameterBuilderFactory Instance { get; } = new();

    private DynamicParameterBuilderFactory()
    {
    }

    public bool IsMatch(Type type)
    {
        return typeof(IDynamicParameter).IsAssignableFrom(type);
    }

    public ParameterBuilder CreateBuilder(ISqlMapperConfig config, Type type)
    {
        return new(
            (cmd, parameter) => ((IDynamicParameter)parameter).Build(config, cmd),
            null);
    }
}
