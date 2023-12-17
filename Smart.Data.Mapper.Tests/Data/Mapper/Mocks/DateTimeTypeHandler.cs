namespace Smart.Data.Mapper.Mocks;

using System.Data;

using Smart.Data.Mapper.Handlers;

public sealed class DateTimeTypeHandler : TypeHandler<DateTime>
{
#pragma warning disable CA1062
    public override void SetValue(IDbDataParameter parameter, DateTime value)
    {
        parameter.DbType = DbType.Int64;
        parameter.Value = value.Ticks;
    }
#pragma warning restore CA1062

    public override DateTime Parse(object value)
    {
        return new((long)value);
    }
}
