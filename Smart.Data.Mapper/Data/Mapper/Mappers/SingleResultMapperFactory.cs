namespace Smart.Data.Mapper.Mappers;

using System;
using System.Collections.Generic;
using System.Data;

public sealed class SingleResultMapperFactory : IResultMapperFactory
{
    private static IEnumerable<Type> SupportedTypes { get; } = new[]
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
        : this(SupportedTypes)
    {
    }

    public SingleResultMapperFactory(IEnumerable<Type> types)
    {
        supportedTypes = new HashSet<Type>(types);
    }

    public bool IsMatch(Type type)
    {
        var targetType = type.IsNullableType() ? Nullable.GetUnderlyingType(type) : type;
        return supportedTypes.Contains(targetType!);
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
    public ResultMapper<T> CreateMapper<T>(ISqlMapperConfig config, Type type, ColumnInfo[] columns)
    {
        var parser = config.CreateParser(columns[0].Type, typeof(T));
        return parser is null
            ? new Mapper<T>()
            : new ParserMapper<T>(parser);
    }

    private sealed class Mapper<T> : ResultMapper<T>
    {
        public override T Map(IDataRecord record)
        {
            var value = record.GetValue(0);
            return value is DBNull ? default! : (T)value;
        }
    }

    private sealed class ParserMapper<T> : ResultMapper<T>
    {
        private readonly Func<object, object> parser;

        public ParserMapper(Func<object, object> parser)
        {
            this.parser = parser;
        }

        public override T Map(IDataRecord record)
        {
            var value = record.GetValue(0);
            return value is DBNull ? default! : (T)parser(value);
        }
    }
}
