namespace Smart.Data.Mapper.Mappers;

using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

public sealed class SingleResultMapperFactory : IResultMapperFactory
{
    private static IEnumerable<Type> SupportedTypes { get; } =
    [
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
    ];

    private readonly HashSet<Type> supportedTypes;

    public SingleResultMapperFactory()
        : this(SupportedTypes)
    {
    }

    public SingleResultMapperFactory(IEnumerable<Type> types)
    {
#pragma warning disable IDE0055
        supportedTypes = [..types];
#pragma warning restore IDE0055
    }

    public bool IsMatch(Type type)
    {
        var targetType = type.IsNullableType() ? Nullable.GetUnderlyingType(type) : type;
        return supportedTypes.Contains(targetType!);
    }

#pragma warning disable CA1062
    public ResultMapper<T> CreateMapper<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.PublicConstructors)] T>(ISqlMapperConfig config, Type type, ColumnInfo[] columns)
    {
        var parser = config.CreateParser(columns[0].Type, typeof(T));
        return parser is null
            ? new Mapper<T>()
            : new ParserMapper<T>(parser);
    }
#pragma warning restore CA1062

    private sealed class Mapper<T> : ResultMapper<T>
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override T Map(IDataRecord record)
        {
            var value = record.GetValue(0);
            return value is DBNull ? default! : UnsafeCastHelper.UnsafeCast<T>(value);
        }
    }

    private sealed class ParserMapper<T> : ResultMapper<T>
    {
        private readonly Func<object, object> parser;

        public ParserMapper(Func<object, object> parser)
        {
            this.parser = parser;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override T Map(IDataRecord record)
        {
            var value = record.GetValue(0);
            return value is DBNull ? default! : UnsafeCastHelper.UnsafeCast<T>(parser(value));
        }
    }
}
