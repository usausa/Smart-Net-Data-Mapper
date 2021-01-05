namespace Smart.Data.Mapper.Builders.Metadata
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Smart.Data.Mapper.Attributes;

    public sealed class StandardTableInfoProvider : ITableInfoProvider
    {
        public static StandardTableInfoProvider Default { get; } = new();

        public IList<string> RemoveSuffix { get; } = new List<string>(new[] { "Entity" });

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
        public TableInfo Create(Type type)
        {
            var columns = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(IsTargetProperty)
                .Select(x => new ColumnInfo(x, x.GetCustomAttribute<NameAttribute>()?.Name ?? x.Name))
                .ToArray();
            var keyColumns = columns
                .Select(x => new { Column = x, Attribute = x.Property.GetCustomAttribute<PrimaryKeyAttribute>() })
                .Where(x => x.Attribute is not null)
                .OrderBy(x => x.Attribute!.Order)
                .Select(x => x.Column)
                .ToArray();
            var nonKeyColumns = columns
                .Where(x => x.Property.GetCustomAttribute<PrimaryKeyAttribute>() is null)
                .ToArray();

            return new TableInfo(ResolveName(type), columns, keyColumns, nonKeyColumns);
        }

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
                    return name.Substring(0, name.Length - suffix.Length);
                }
            }

            return name;
        }
    }
}
