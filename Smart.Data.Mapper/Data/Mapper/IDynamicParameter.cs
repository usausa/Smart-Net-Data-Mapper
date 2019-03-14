namespace Smart.Data.Mapper
{
    using System.Data;

    public interface IDynamicParameter
    {
        void Build(ISqlMapperConfig config, IDbCommand cmd);
    }
}
