namespace Smart.Data.Mapper.Parameters
{
    using System;
    using System.Collections.Generic;

    public sealed class DictionaryParameterBuilderFactory : IParameterBuilderFactory
    {
        public static DictionaryParameterBuilderFactory Instance { get; } = new DictionaryParameterBuilderFactory();

        private DictionaryParameterBuilderFactory()
        {
        }

        public bool IsMatch(Type type)
        {
            return typeof(IDictionary<string, object>).IsAssignableFrom(type);
        }

        public ParameterBuilder CreateBuilder(ISqlMapperConfig config, Type type)
        {
            return new ParameterBuilder(
                (cmd, parameter) =>
                {
                    foreach (var keyValue in (IDictionary<string, object>)parameter)
                    {
                        var param = cmd.CreateParameter();
                        param.ParameterName = keyValue.Key;

                        var value = keyValue.Value;
                        if (value is null)
                        {
                            param.Value = DBNull.Value;
                        }
                        else
                        {
                            var entry = config.LookupTypeHandle(value.GetType());
                            param.DbType = entry.DbType;

                            if (entry.TypeHandler != null)
                            {
                                entry.TypeHandler.SetValue(param, value);
                            }
                            else
                            {
                                param.Value = value;
                            }
                        }

                        cmd.Parameters.Add(param);
                    }
                },
                null);
        }
    }
}
