namespace Smart.Data.Mapper.Mappers;

using System.Data;
using System.Reflection;

using Smart.Data.Mapper.Attributes;

public sealed class ObjectResultMapperFactory : IResultMapperFactory
{
    public static ObjectResultMapperFactory Instance { get; } = new();

    private ObjectResultMapperFactory()
    {
    }

    public bool IsMatch(Type type) => true;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
    public ResultMapper<T> CreateMapper<T>(ISqlMapperConfig config, Type type, ColumnInfo[] columns)
    {
        return new Mapper<T>(config.CreateFactory<T>(), CreateMapEntries(config, type, columns));
    }

    private static MapEntry[] CreateMapEntries(ISqlMapperConfig config, Type type, ColumnInfo[] columns)
    {
        var selector = config.GetPropertySelector();
        var properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(IsTargetProperty)
            .ToArray();

        var list = new List<MapEntry>();
        for (var i = 0; i < columns.Length; i++)
        {
            var column = columns[i];
            var pi = selector(properties, column.Name);
            if (pi is null)
            {
                continue;
            }
            var setter = config.CreateSetter(pi);
            var defaultValue = pi.PropertyType.GetDefaultValue();

            var parser = config.CreateParser(column.Type, pi.PropertyType);
            list.Add(parser is null
                ? new MapEntry(i, CreateParser(setter!, defaultValue))
                : new MapEntry(i, CreateParser(setter!, defaultValue, parser)));
        }

        return list.ToArray();
    }

    private static Action<object, object> CreateParser(Action<object?, object?> setter, object? defaultValue)
    {
        return (obj, value) => setter(obj, value is DBNull ? defaultValue : value);
    }

    private static Action<object, object> CreateParser(Action<object?, object?> setter, object? defaultValue, Func<object, object> parser)
    {
        return (obj, value) => setter(obj, value is DBNull ? defaultValue : parser(value));
    }

    private static bool IsTargetProperty(PropertyInfo pi)
    {
        return pi.CanWrite && (pi.GetCustomAttribute<IgnoreAttribute>() is null);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Performance")]
    private sealed class MapEntry
    {
        public readonly int Index;

        public readonly Action<object, object> Setter;

        public MapEntry(int index, Action<object, object> setter)
        {
            Index = index;
            Setter = setter;
        }
    }

    private sealed class Mapper<T> : ResultMapper<T>
    {
        private readonly Func<T> factory;

        private readonly MapEntry[] entries;

        public Mapper(Func<T> factory, MapEntry[] entries)
        {
            this.factory = factory;
            this.entries = entries;
        }

        public override T Map(IDataRecord record)
        {
            var obj = factory();

            foreach (var entry in entries)
            {
                entry.Setter(obj!, record.GetValue(entry.Index));
            }

            return obj;
        }
    }
}
