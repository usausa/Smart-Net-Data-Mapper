namespace Smart.Data.Mapper.Builders;

using System.Text;

using Smart.Data.Mapper.Builders.Metadata;

public static class SqlUpdate<T>
{
    private static readonly string ByKeySql;

    private static readonly string UpdateSql;

    private static readonly string KeyConditionSql;

    static SqlUpdate()
    {
        var tableInfo = TableInfo<T>.Instance;
        var sql = new StringBuilder(256);

        UpdateSql = $"UPDATE {tableInfo.Name} SET ";

        if ((tableInfo.KeyColumns.Count > 0) &&
            (tableInfo.NonKeyColumns.Count > 0))
        {
            sql.Append(" WHERE ");
            BuildHelper.BuildKeyCondition(sql, tableInfo);

            KeyConditionSql = sql.ToString();

            sql.Clear();
            sql.Append(UpdateSql);
            foreach (var column in tableInfo.NonKeyColumns)
            {
                sql.Append(column.Name);
                sql.Append(" = ");
                sql.Append('@');
                sql.Append(column.Property.Name);
                sql.Append(", ");
            }
            sql.Length -= 2;
            sql.Append(KeyConditionSql);

            ByKeySql = sql.ToString();
        }
        else
        {
            KeyConditionSql = string.Empty;
            ByKeySql = string.Empty;
        }
    }

    public static string ByKey() => ByKeySql;

    public static string ByKey(string set) => !String.IsNullOrEmpty(KeyConditionSql) ? String.Concat(UpdateSql, set, KeyConditionSql) : string.Empty;

    public static string Set(string set, string condition) => String.Concat(UpdateSql, set, " WHERE ", condition);
}
