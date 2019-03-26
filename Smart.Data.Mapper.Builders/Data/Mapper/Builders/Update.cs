namespace Smart.Data.Mapper.Builders
{
    using System;
    using System.Text;

    using Smart.Data.Mapper.Builders.Metadata;

    public static class Update<T>
    {
        private static readonly string ByKeySql;

        private static readonly string UpdateSql;

        private static readonly string KeyConditionSql;

        static Update()
        {
            var tableInfo = Metadata<T>.Table;
            var sql = new StringBuilder(256);

            UpdateSql = $"UPDATE {tableInfo} SET ";

            sql.Append(" WHERE ");
            BuildHelper.BuildKeyCondition(sql, tableInfo);

            KeyConditionSql = sql.ToString();

            sql.Clear();
            sql.Append(UpdateSql);
            foreach (var column in tableInfo.NonKeyColumns)
            {
                sql.Append(column.Name);
                sql.Append(" = ");
                sql.Append("@");
                sql.Append(column.Property.Name);
                sql.Append(", ");
            }
            sql.Append(KeyConditionSql);

            ByKeySql = sql.ToString();
        }

        public static string SetByKey() => ByKeySql;

        public static string SetByKey(string set) => String.Concat(UpdateSql, set, KeyConditionSql);

        public static string SetBy(string set, string condition) => String.Concat(UpdateSql, set, " WHERE ", condition);
    }
}
