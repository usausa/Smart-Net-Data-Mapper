namespace Smart.Data.Mapper.Builders.Metadata;

using System.Diagnostics.CodeAnalysis;

public interface ITableMetadataProvider
{
    TableMetadata Create([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type type);
}
