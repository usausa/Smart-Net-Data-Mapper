namespace Smart.Data.Mapper.Builders;

using System.Text;

using Smart.Data.Mapper.Builders.Metadata;

#pragma warning disable CA1000
#pragma warning disable CA1810
public static class SqlInsert<T>
{
    private static readonly string ValuesSql;

    static SqlInsert()
    {
        var tableInfo = TableInfo<T>.Instance;
        var sql = new StringBuilder(256);

        sql.Append("INSERT INTO ");
        sql.Append(tableInfo.Name);
        sql.Append(" (");
        foreach (var column in tableInfo.Columns)
        {
            sql.Append(column.Name);
            sql.Append(", ");
        }
        sql.Length -= 2;
        sql.Append(") VALUES (");
        foreach (var column in tableInfo.Columns)
        {
            sql.Append('@');
            sql.Append(column.Property.Name);
            sql.Append(", ");
        }
        sql.Length -= 2;
        sql.Append(')');

        ValuesSql = sql.ToString();
    }

    public static string Values() => ValuesSql;
}
#pragma warning restore CA1810
#pragma warning restore CA1000
