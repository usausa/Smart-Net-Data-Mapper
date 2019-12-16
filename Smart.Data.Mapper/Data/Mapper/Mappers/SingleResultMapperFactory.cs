namespace Smart.Data.Mapper.Mappers
{
    using System;
    using System.Collections.Generic;
    using System.Data;

    public sealed class SingleResultMapperFactory : IResultMapperFactory
    {
        public static IEnumerable<Type> SupportedTypes { get; } = new[]
        {
            typeof(byte),
            typeof(sbyte),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),
            typeof(float),
            typeof(double),
            typeof(decimal),
            typeof(bool),
            typeof(string),
            typeof(char),
            typeof(Guid),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(byte[])
        };

        private readonly HashSet<Type> supportedTypes;

        public SingleResultMapperFactory()
            : this(SingleResultMapperFactory.SupportedTypes)
        {
        }

        public SingleResultMapperFactory(IEnumerable<Type> types)
        {
            this.supportedTypes = new HashSet<Type>(types);
        }

        public bool IsMatch(Type type)
        {
            return supportedTypes.Contains(type.IsNullableType() ? Nullable.GetUnderlyingType(type) : type);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
        public Func<IDataRecord, T> CreateMapper<T>(ISqlMapperConfig config, Type type, ColumnInfo[] columns)
        {
            var defaultValue = default(T);
            var parser = config.CreateParser(columns[0].Type, typeof(T));
            return parser != null
                ? CreateConvertMapper(defaultValue, parser)
                : CreateConvertMapper(defaultValue);
        }

        private static Func<IDataRecord, T> CreateConvertMapper<T>(T defaultValue)
        {
            return record =>
            {
                object value = record.GetValue(0);
                return value is DBNull ? defaultValue : (T)value;
            };
        }

        private static Func<IDataRecord, T> CreateConvertMapper<T>(T defaultValue, Func<object, object> parser)
        {
            return record =>
            {
                object value = record.GetValue(0);
                return value is DBNull ? defaultValue : (T)parser(value);
            };
        }
    }
}
