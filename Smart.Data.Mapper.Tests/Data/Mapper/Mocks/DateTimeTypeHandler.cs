namespace Smart.Data.Mapper.Mocks;

using System;
using System.Data;

using Smart.Data.Mapper.Handlers;

public sealed class DateTimeTypeHandler : TypeHandler<DateTime>
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Ignore")]
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.DbType = DbType.Int64;
        parameter.Value = value.Ticks;
    }

    public override DateTime Parse(object value)
    {
        return new((long)value);
    }
}
