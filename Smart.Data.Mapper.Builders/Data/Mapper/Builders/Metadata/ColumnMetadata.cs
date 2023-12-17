namespace Smart.Data.Mapper.Builders.Metadata;

using System.Reflection;

public sealed class ColumnMetadata
{
    public PropertyInfo Property { get; }

    public string Name { get; }

    public ColumnMetadata(PropertyInfo property, string name)
    {
        Property = property;
        Name = name;
    }
}
