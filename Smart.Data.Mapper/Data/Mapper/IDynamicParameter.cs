namespace Smart.Data.Mapper;

using System.Data.Common;

public interface IDynamicParameter
{
    void Build(ISqlMapperConfig config, DbCommand cmd);
}
