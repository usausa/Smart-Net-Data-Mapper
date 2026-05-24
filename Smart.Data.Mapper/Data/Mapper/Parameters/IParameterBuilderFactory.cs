namespace Smart.Data.Mapper.Parameters;

using System.Diagnostics.CodeAnalysis;

public interface IParameterBuilderFactory
{
    bool IsMatch(Type type);

    ParameterBuilder CreateBuilder(ISqlMapperConfig config, [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicConstructors)] Type type);
}
