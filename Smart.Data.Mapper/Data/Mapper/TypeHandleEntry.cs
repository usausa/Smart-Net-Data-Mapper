namespace Smart.Data.Mapper
{
    using System.Data;

    using Smart.Data.Mapper.Handlers;

    public sealed class TypeHandleEntry
    {
        public bool CanUseAsParameter { get; }

        public DbType DbType { get; }

        public ITypeHandler TypeHandler { get; }

        public TypeHandleEntry(bool canUseAsParameter, DbType dbType, ITypeHandler typeHandler)
        {
            CanUseAsParameter = canUseAsParameter;
            DbType = dbType;
            TypeHandler = typeHandler;
        }
    }
}
