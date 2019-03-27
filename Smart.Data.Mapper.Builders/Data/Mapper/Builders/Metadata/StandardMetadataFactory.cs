namespace Smart.Data.Mapper.Builders.Metadata
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Smart.Data.Mapper.Attributes;

    public sealed class StandardMetadataFactory : IMetadataFactory
    {
        public static StandardMetadataFactory Default { get; } = new StandardMetadataFactory();

        public string[] RemoveSuffix { get; set; } = { "Entity" };

        public TableInfo CreateTableInfo(Type type)
        {
            var columns = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(IsTargetProperty)
                .Select(x => new ColumnInfo(x, x.GetCustomAttribute<NameAttribute>()?.Name ?? x.Name))
                .ToArray();
            var keyColumns = columns
                .Select(x => new { Column = x, Attribute = x.Property.GetCustomAttribute<PrimaryKeyAttribute>() })
                .Where(x => x.Attribute != null)
                .OrderBy(x => x.Attribute.Order)
                .Select(x => x.Column)
                .ToArray();
            var nonKeyColumns = columns
                .Where(x => x.Property.GetCustomAttribute<PrimaryKeyAttribute>() == null)
                .ToArray();

            return new TableInfo(ResolveName(type), columns, keyColumns, nonKeyColumns);
        }

        private static bool IsTargetProperty(PropertyInfo pi)
        {
            return pi.CanRead && (pi.GetCustomAttribute<IgnoreAttribute>() == null);
        }

        private string ResolveName(MemberInfo mi)
        {
            var attribute = mi.GetCustomAttribute<NameAttribute>();
            if (attribute != null)
            {
                return attribute.Name;
            }

            var name = mi.Name;
            foreach (var suffix in RemoveSuffix)
            {
                if (name.EndsWith(suffix))
                {
                    return name.Substring(0, name.Length - suffix.Length);
                }
            }

            return name;
        }
    }
}
