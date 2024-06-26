namespace Smart.Data.Mapper.Builders.Metadata;

using System.Reflection;

using Smart.Data.Mapper.Attributes;

public sealed class StandardTableInfoProvider : ITableMetadataProvider
{
    public static StandardTableInfoProvider Default { get; } = new();

    public IList<string> RemoveSuffix { get; } = new List<string>(["Entity"]);

#pragma warning disable CA1062
    public TableMetadata Create(Type type)
    {
        var columns = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(IsTargetProperty)
            .Select(static x => new ColumnMetadata(x, x.GetCustomAttribute<NameAttribute>()?.Name ?? x.Name))
            .ToArray();
        var keyColumns = columns
            .Select(static x => new { Column = x, Attribute = x.Property.GetCustomAttribute<PrimaryKeyAttribute>() })
            .Where(static x => x.Attribute is not null)
            .OrderBy(static x => x.Attribute!.Order)
            .Select(static x => x.Column)
            .ToArray();
        var nonKeyColumns = columns
            .Where(static x => x.Property.GetCustomAttribute<PrimaryKeyAttribute>() is null)
            .ToArray();

        return new TableMetadata(ResolveName(type), columns, keyColumns, nonKeyColumns);
    }
#pragma warning restore CA1062

    private static bool IsTargetProperty(PropertyInfo pi)
    {
        return pi.CanRead && (pi.GetCustomAttribute<IgnoreAttribute>() is null);
    }

    private string ResolveName(MemberInfo mi)
    {
        var attribute = mi.GetCustomAttribute<NameAttribute>();
        if (attribute is not null)
        {
            return attribute.Name;
        }

        var name = mi.Name;
        foreach (var suffix in RemoveSuffix)
        {
            if (name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase))
            {
                return name[..^suffix.Length];
            }
        }

        return name;
    }
}
