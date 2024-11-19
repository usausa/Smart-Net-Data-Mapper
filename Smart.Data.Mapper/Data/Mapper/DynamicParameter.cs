namespace Smart.Data.Mapper;

using System.Data;
using System.Data.Common;

public sealed class DynamicParameter : IDynamicParameter
{
    private readonly Dictionary<string, ParameterInfo> parameters = [];

#pragma warning disable SA1401
    private sealed class ParameterInfo
    {
        public readonly string Name;

        public readonly object? Value;

        public readonly DbType? DbType;

        public readonly int? Size;

        public readonly ParameterDirection Direction;

        public DbParameter? AttachedParam;

        public ParameterInfo(string name, object? value, DbType? dbType, int? size, ParameterDirection direction)
        {
            Name = name;
            Value = value;
            DbType = dbType;
            Size = size;
            Direction = direction;
        }
    }
#pragma warning restore SA1401

    public void Add(string name, object? value, DbType? dbType = null, int? size = null, ParameterDirection direction = ParameterDirection.Input)
    {
        parameters[name] = new ParameterInfo(name, value, dbType, size, direction);
    }

    public T Get<T>(string name)
    {
        var value = parameters[name].AttachedParam?.Value ?? DBNull.Value;
        if (value == DBNull.Value)
        {
            return default!;
        }

        return (T)value;
    }

#pragma warning disable CA1062
    public void Build(ISqlMapperConfig config, DbCommand cmd)
    {
        foreach (var parameter in parameters.Values)
        {
            var param = cmd.CreateParameter();
            param.ParameterName = parameter.Name;

            param.Direction = parameter.Direction;

            if ((parameter.Direction == ParameterDirection.Input) ||
                (parameter.Direction == ParameterDirection.InputOutput))
            {
                var value = parameter.Value;
                if (value is null)
                {
                    param.Value = DBNull.Value;
                }
                else
                {
                    var entry = config.LookupTypeHandle(value.GetType());
                    param.DbType = parameter.DbType ?? entry.DbType;

                    if (parameter.Size.HasValue)
                    {
                        param.Size = parameter.Size.Value;
                    }

                    if (entry.TypeHandler is not null)
                    {
                        entry.TypeHandler.SetValue(param, value);
                    }
                    else
                    {
                        param.Value = value;
                    }
                }
            }

            cmd.Parameters.Add(param);
            parameter.AttachedParam = param;
        }
    }
#pragma warning restore CA1062
}
