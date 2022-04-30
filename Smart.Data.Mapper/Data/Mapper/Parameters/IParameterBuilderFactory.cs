namespace Smart.Data.Mapper.Parameters;

public interface IParameterBuilderFactory
{
    bool IsMatch(Type type);

    ParameterBuilder CreateBuilder(ISqlMapperConfig config, Type type);
}
