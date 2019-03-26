namespace Smart.Data.Mapper.Builders.Metadata
{
    using System.Reflection;

    public class ColumnInfo
    {
        public PropertyInfo Property { get; }

        public string Name { get; }

        public ColumnInfo(PropertyInfo property, string name)
        {
            Property = property;
            Name = name;
        }
    }
}
