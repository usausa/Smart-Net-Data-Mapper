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
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public);
        var columns = new List<ColumnMetadata>(properties.Length);
        var keyColumns = new List<(ColumnMetadata Column, int Order)>();
        var nonKeyColumns = new List<ColumnMetadata>(properties.Length);

        for (var i = 0; i < properties.Length; i++)
        {
            var property = properties[i];
            if (!IsTargetProperty(property))
            {
                continue;
            }

            var column = new ColumnMetadata(property, property.GetCustomAttribute<NameAttribute>()?.Name ?? property.Name);
            columns.Add(column);

            var primaryKey = property.GetCustomAttribute<PrimaryKeyAttribute>();
            if (primaryKey is null)
            {
                nonKeyColumns.Add(column);
            }
            else
            {
                keyColumns.Add((column, primaryKey.Order));
            }
        }

        keyColumns.Sort(static (x, y) => x.Order.CompareTo(y.Order));

        return new TableMetadata(
            ResolveName(type),
            [.. columns],
            keyColumns.ConvertAll(static x => x.Column),
            [.. nonKeyColumns]);
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
