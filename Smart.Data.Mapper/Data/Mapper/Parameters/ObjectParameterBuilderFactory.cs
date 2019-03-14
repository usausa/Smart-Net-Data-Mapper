namespace Smart.Data.Mapper.Parameters
{
    using System;
    using System.Data;
    using System.Linq;
    using System.Reflection;

    using Smart.Data.Mapper.Attributes;
    using Smart.Data.Mapper.Handlers;

    public sealed class ObjectParameterBuilderFactory : IParameterBuilderFactory
    {
        public static ObjectParameterBuilderFactory Instance { get; } = new ObjectParameterBuilderFactory();

        private ObjectParameterBuilderFactory()
        {
        }

        public bool IsMatch(Type type) => true;

        public ParameterBuilder CreateBuilder(ISqlMapperConfig config, Type type)
        {
            var entries = CreateParameterEntries(config, type);

            return new ParameterBuilder(CreateBuildAction(entries), CreatePostProcessAction(entries));
        }

        private static ParameterEntry[] CreateParameterEntries(ISqlMapperConfig config, Type type)
        {
            return type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                .Where(IsTargetProperty)
                .Select(x =>
                {
                    var dbType = x.GetCustomAttribute<DbTypeAttribute>()?.DbType;
                    var size = x.GetCustomAttribute<SizeAttribute>()?.Size;
                    var direction = x.GetCustomAttribute<DirectionAttribute>()?.Direction ?? ParameterDirection.Input;
                    var entry = config.LookupTypeHandle(x.PropertyType);
                    var getter = ((direction == ParameterDirection.Input) || (direction == ParameterDirection.InputOutput)) ? config.CreateGetter(x) : null;
                    var setter = direction != ParameterDirection.Input ? CreateConvertSetter(config, x) : null;
                    return new ParameterEntry(x.Name, getter, setter, dbType ?? entry.DbType, entry.TypeHandler, size, direction);
                })
                .ToArray();
        }

        private static bool IsTargetProperty(PropertyInfo pi)
        {
            return pi.CanRead && (pi.GetCustomAttribute<IgnoreAttribute>() == null);
        }

        private static Action<object, object> CreateConvertSetter(ISqlMapperConfig config, PropertyInfo pi)
        {
            var setter = config.CreateSetter(pi);
            var defaultValue = pi.PropertyType.GetDefaultValue();
            var destinationType = pi.PropertyType;

            return (target, value) =>
            {
                if (value is DBNull)
                {
                    setter(target, defaultValue);
                }
                else if (value.GetType() == destinationType)
                {
                    setter(target, value);
                }
                else
                {
                    var parser = config.CreateParser(value.GetType(), destinationType);
                    setter(target, parser(value));
                }
            };
        }

        private static Action<IDbCommand, object> CreateBuildAction(ParameterEntry[] entries)
        {
            return (cmd, parameter) =>
            {
                for (var i = 0; i < entries.Length; i++)
                {
                    var entry = entries[i];
                    var param = cmd.CreateParameter();

                    param.ParameterName = entry.Name;

                    param.Direction = entry.Direction;

                    if (entry.Getter != null)
                    {
                        var value = entry.Getter(parameter);
                        if (value is null)
                        {
                            param.Value = DBNull.Value;
                        }
                        else
                        {
                            param.DbType = entry.DbType;

                            if (entry.Size.HasValue)
                            {
                                param.Size = entry.Size.Value;
                            }

                            if (entry.Handler != null)
                            {
                                entry.Handler.SetValue(param, value);
                            }
                            else
                            {
                                param.Value = value;
                            }
                        }
                    }

                    cmd.Parameters.Add(param);
                }
            };
        }

        private static Action<IDbCommand, object> CreatePostProcessAction(ParameterEntry[] entries)
        {
            if (entries.Any(x => x.Setter != null))
            {
                return (cmd, parameter) =>
                {
                    for (var i = 0; i < entries.Length; i++)
                    {
                        entries[i].Setter?.Invoke(parameter, ((IDbDataParameter)cmd.Parameters[i]).Value);
                    }
                };
            }

            return null;
        }

        private sealed class ParameterEntry
        {
            public string Name { get; }

            public Func<object, object> Getter { get; }

            public Action<object, object> Setter { get; }

            public DbType DbType { get; }

            public ITypeHandler Handler { get; }

            public int? Size { get; }

            public ParameterDirection Direction { get; }

            public ParameterEntry(string name, Func<object, object> getter, Action<object, object> setter, DbType dbType, ITypeHandler handler, int? size, ParameterDirection direction)
            {
                Name = name;
                Getter = getter;
                Setter = setter;
                DbType = dbType;
                Handler = handler;
                Size = size;
                Direction = direction;
            }
        }
    }
}
