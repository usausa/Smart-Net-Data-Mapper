namespace Smart.Data.Mapper.Builder.Metadata
{
    using System;
    using System.Linq;
    using System.Reflection;

    using Smart.Data.Mapper.Attributes;

    public sealed class DefaultMetadataFactory : IMetadataFactory
    {
        public static DefaultMetadataFactory Instance { get; } = new DefaultMetadataFactory();

        private DefaultMetadataFactory()
        {
        }

        public TableInfo CreateTableInfo(Type type)
        {
            var columns = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(IsTargetProperty)
                .Select(x => new ColumnInfo(x, ResolveName(x)))
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

        private static string ResolveName(MemberInfo pi)
        {
            return pi.GetCustomAttribute<NameAttribute>()?.Name ?? pi.Name;
        }
    }
}
