namespace Smart.Data.Mapper.Mocks;

using Smart.Data.Mapper.Parameters;

public sealed class MockParameterBuilderFactory : IParameterBuilderFactory
{
    public bool BuildCalled { get; private set; }

    public bool PostProcessCalled { get; private set; }

    public bool IsMatch(Type type) => true;

    public ParameterBuilder CreateBuilder(ISqlMapperConfig config, Type type)
    {
        return new((_, _) => { BuildCalled = true; }, (_, _) => { PostProcessCalled = true; });
    }
}
