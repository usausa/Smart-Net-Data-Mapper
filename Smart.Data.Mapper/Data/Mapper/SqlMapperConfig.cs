namespace Smart.Data.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Reflection;

    using Smart.Collections.Concurrent;
    using Smart.Converter;
    using Smart.Data.Mapper.Handlers;
    using Smart.Data.Mapper.Mappers;
    using Smart.Data.Mapper.Parameters;
    using Smart.Data.Mapper.Selector;
    using Smart.Reflection;

    public sealed class SqlMapperConfig : ISqlMapperConfig
    {
        private static readonly IParameterBuilderFactory[] DefaultParameterBuilderFactories =
        {
            DynamicParameterBuilderFactory.Instance,
            DictionaryParameterBuilderFactory.Instance,
            ObjectParameterBuilderFactory.Instance
        };

        private static readonly IResultMapperFactory[] DefaultResultMapperFactories =
        {
            ObjectResultMapperFactory.Instance
        };

        private static readonly Dictionary<Type, DbType> DefaultTypeMap = new Dictionary<Type, DbType>
        {
            { typeof(byte), DbType.Byte },
            { typeof(sbyte), DbType.SByte },
            { typeof(short), DbType.Int16 },
            { typeof(ushort), DbType.UInt16 },
            { typeof(int), DbType.Int32 },
            { typeof(uint), DbType.UInt32 },
            { typeof(long), DbType.Int64 },
            { typeof(ulong), DbType.UInt64 },
            { typeof(float), DbType.Single },
            { typeof(double), DbType.Double },
            { typeof(decimal), DbType.Decimal },
            { typeof(bool), DbType.Boolean },
            { typeof(string), DbType.String },
            { typeof(char), DbType.StringFixedLength },
            { typeof(Guid), DbType.Guid },
            { typeof(DateTime), DbType.DateTime },
            { typeof(DateTimeOffset), DbType.DateTimeOffset },
            { typeof(TimeSpan), DbType.Time },
            { typeof(byte[]), DbType.Binary },
            { typeof(object), DbType.Object },
        };

        private static readonly Dictionary<Type, ITypeHandler> DefaultTypeHandlers = new Dictionary<Type, ITypeHandler>();

        [ThreadStatic]
        private static ColumnInfo[] columnInfoPool;

        private readonly ThreadsafeTypeHashArrayMap<ParameterBuilder> parameterBuilderCache = new ThreadsafeTypeHashArrayMap<ParameterBuilder>();

        private readonly ResultMapperCache resultMapperCache = new ResultMapperCache();

        private readonly ThreadsafeTypeHashArrayMap<TypeHandleEntry> typeHandleEntriesCache = new ThreadsafeTypeHashArrayMap<TypeHandleEntry>();

        private IParameterBuilderFactory[] parameterBuilderFactories;

        private IResultMapperFactory[] resultMapperFactories;

        private Dictionary<Type, DbType> typeMap;

        private Dictionary<Type, ITypeHandler> typeHandlers;

        //--------------------------------------------------------------------------------
        // Property
        //--------------------------------------------------------------------------------

        public static SqlMapperConfig Default { get; } = new SqlMapperConfig();

        public IDelegateFactory DelegateFactory { get; set; } = Smart.Reflection.DelegateFactory.Default;

        public IObjectConverter Converter { get; set; } = ObjectConverter.Default;

        public IPropertySelector PropertySelector { get; set; } = DefaultPropertySelector.Instance;

        //--------------------------------------------------------------------------------
        // Constructor
        //--------------------------------------------------------------------------------

        public SqlMapperConfig()
        {
            parameterBuilderFactories = DefaultParameterBuilderFactories;
            resultMapperFactories = DefaultResultMapperFactories;
            typeMap = DefaultTypeMap;
            typeHandlers = DefaultTypeHandlers;
        }

        //--------------------------------------------------------------------------------
        // Cache
        //--------------------------------------------------------------------------------

        public int CountParameterBuilderCache => parameterBuilderCache.Count;

        public int CountResultMapperCache => resultMapperCache.Count;

        public int CountTypeHandleEntriesCache => typeHandleEntriesCache.Count;

        public void ClearParameterBuilderCache() => parameterBuilderCache.Clear();

        public void ClearResultMapperCache() => resultMapperCache.Clear();

        public void ClearTypeHandleEntriesCache() => typeHandleEntriesCache.Clear();

        //--------------------------------------------------------------------------------
        // Config
        //--------------------------------------------------------------------------------

        public SqlMapperConfig ResetParameterBuilderFactories()
        {
            parameterBuilderFactories = DefaultParameterBuilderFactories;
            parameterBuilderCache.Clear();
            return this;
        }

        public SqlMapperConfig ConfigureParameterBuilderFactories(Action<IList<IParameterBuilderFactory>> action)
        {
            var list = new List<IParameterBuilderFactory>(parameterBuilderFactories);
            action(list);
            parameterBuilderFactories = list.ToArray();
            parameterBuilderCache.Clear();
            return this;
        }

        public SqlMapperConfig ResetResultMapperFactories()
        {
            resultMapperFactories = DefaultResultMapperFactories;
            resultMapperCache.Clear();
            return this;
        }

        public SqlMapperConfig ConfigureResultMapperFactories(Action<IList<IResultMapperFactory>> action)
        {
            var list = new List<IResultMapperFactory>(resultMapperFactories);
            action(list);
            resultMapperFactories = list.ToArray();
            resultMapperCache.Clear();
            return this;
        }

        public SqlMapperConfig ResetTypeMap()
        {
            typeMap = DefaultTypeMap;
            typeHandleEntriesCache.Clear();
            return this;
        }

        public SqlMapperConfig ConfigureTypeMap(Action<IDictionary<Type, DbType>> action)
        {
            var dictionary = new Dictionary<Type, DbType>(typeMap);
            action(dictionary);
            typeMap = dictionary;
            typeHandleEntriesCache.Clear();
            return this;
        }

        public SqlMapperConfig ResetTypeHandlers()
        {
            typeHandlers = DefaultTypeHandlers;
            typeHandleEntriesCache.Clear();
            return this;
        }

        public SqlMapperConfig ConfigureTypeHandlers(Action<IDictionary<Type, ITypeHandler>> action)
        {
            var dictionary = new Dictionary<Type, ITypeHandler>(typeHandlers);
            action(dictionary);
            typeHandlers = dictionary;
            typeHandleEntriesCache.Clear();
            return this;
        }

        //--------------------------------------------------------------------------------
        // ISqlMapperConfig
        //--------------------------------------------------------------------------------

        Func<T> ISqlMapperConfig.CreateFactory<T>() => DelegateFactory.CreateFactory<T>();

        Func<object, object> ISqlMapperConfig.CreateGetter(PropertyInfo pi) => DelegateFactory.CreateGetter(pi);

        Action<object, object> ISqlMapperConfig.CreateSetter(PropertyInfo pi) => DelegateFactory.CreateSetter(pi);

        Func<PropertyInfo[], string, PropertyInfo> ISqlMapperConfig.GetPropertySelector() => PropertySelector.SelectProperty;

        Func<object, object> ISqlMapperConfig.CreateParser(Type sourceType, Type destinationType)
        {
            if (!typeHandleEntriesCache.TryGetValue(destinationType, out var entry))
            {
                entry = typeHandleEntriesCache.AddIfNotExist(destinationType, CreateTypeHandleInternal);
            }

            if (entry.TypeHandler != null)
            {
                return x => entry.TypeHandler.Parse(destinationType, x);
            }

            return Converter.CreateConverter(sourceType, destinationType);
        }

        TypeHandleEntry ISqlMapperConfig.LookupTypeHandle(Type type)
        {
            if (!typeHandleEntriesCache.TryGetValue(type, out var entry))
            {
                entry = typeHandleEntriesCache.AddIfNotExist(type, CreateTypeHandleInternal);
            }

            if (!entry.CanUseAsParameter)
            {
                throw new SqlMapperException($"Type cannot use as parameter. type=[{type.FullName}]");
            }

            return entry;
        }

        private TypeHandleEntry CreateTypeHandleInternal(Type type)
        {
            type = Nullable.GetUnderlyingType(type) ?? type;

            var findDbType = typeMap.TryGetValue(type, out var dbType);
            if (!findDbType && type.IsEnum)
            {
                findDbType = typeMap.TryGetValue(Enum.GetUnderlyingType(type), out dbType);
            }

            typeHandlers.TryGetValue(type, out var handler);

            return new TypeHandleEntry(findDbType || (handler != null), dbType, handler);
        }

        Func<IDataRecord, T> ISqlMapperConfig.CreateResultMapper<T>(IDataReader reader)
        {
            var fieldCount = reader.FieldCount;
            if ((columnInfoPool == null) || (columnInfoPool.Length < fieldCount))
            {
                columnInfoPool = new ColumnInfo[fieldCount];
            }

            var type = typeof(T);
            for (var i = 0; i < reader.FieldCount; i++)
            {
                columnInfoPool[i] = new ColumnInfo(reader.GetName(i), reader.GetFieldType(i));
            }

            var columns = new Span<ColumnInfo>(columnInfoPool, 0, fieldCount);

            if (resultMapperCache.TryGetValue(type, columns, out var value))
            {
                return (Func<IDataRecord, T>)value;
            }

            return (Func<IDataRecord, T>)resultMapperCache.AddIfNotExist(type, columns, CreateMapperInternal<T>);
        }

        private object CreateMapperInternal<T>(Type type, ColumnInfo[] columns)
        {
            foreach (var factory in resultMapperFactories)
            {
                if (factory.IsMatch(type))
                {
                    return factory.CreateMapper<T>(this, type, columns);
                }
            }

            throw new SqlMapperException($"Result type is not supported. type=[{type.FullName}]");
        }

        ParameterBuilder ISqlMapperConfig.CreateParameterBuilder(Type type)
        {
            if (!parameterBuilderCache.TryGetValue(type, out var parameterBuilder))
            {
                parameterBuilder = parameterBuilderCache.AddIfNotExist(type, CreateParameterBuilderInternal);
            }

            return parameterBuilder;
        }

        private ParameterBuilder CreateParameterBuilderInternal(Type type)
        {
            foreach (var factory in parameterBuilderFactories)
            {
                if (factory.IsMatch(type))
                {
                    return factory.CreateBuilder(this, type);
                }
            }

            throw new SqlMapperException($"Parameter type is not supported. type=[{type.FullName}]");
        }
    }
}
