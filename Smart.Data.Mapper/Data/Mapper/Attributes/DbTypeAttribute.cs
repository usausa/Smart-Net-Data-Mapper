namespace Smart.Data.Mapper.Attributes
{
    using System;
    using System.Data;

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class DbTypeAttribute : Attribute
    {
        public DbType DbType { get; }

        public DbTypeAttribute(DbType dbType)
        {
            DbType = dbType;
        }
    }
}
