namespace DataAccess.FormsApp.Handlers
{
    using System;
    using System.Data;

    using Smart.Data.Mapper.Handlers;

    public sealed class DateTimeOffsetTypeHandler : TypeHandler<DateTimeOffset>
    {
        public override void SetValue(IDbDataParameter parameter, DateTimeOffset value)
        {
            parameter.DbType = DbType.Int64;
            parameter.Value = value.Ticks;
        }

        public override DateTimeOffset Parse(object value)
        {
            return new DateTimeOffset(new DateTime((long)value));
        }
    }
}
