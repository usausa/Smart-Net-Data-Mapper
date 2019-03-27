namespace Smart.Data.Mapper.Builders
{
    using System;
    using System.Text;

    using Smart.Data.Mapper.Builders.Metadata;

    public static class SqlSelect<T>
    {
        private static readonly string ByKeySql;

        private static readonly string WhereSqlBase;

        private static readonly string OrderSqlBase;

        static SqlSelect()
        {
            var tableInfo = Metadata<T>.Table;
            var sql = new StringBuilder(256);

            sql.Append("SELECT * FROM ");
            sql.Append(tableInfo.Name);
            sql.Append(" WHERE ");

            WhereSqlBase = sql.ToString();

            if (tableInfo.KeyColumns.Count > 0)
            {
                BuildHelper.BuildKeyCondition(sql, tableInfo);

                ByKeySql = sql.ToString();

                sql.Clear();
                sql.Append(" ORDER BY ");
                foreach (var column in tableInfo.KeyColumns)
                {
                    sql.Append(column.Name);
                    sql.Append(", ");
                }

                sql.Length = sql.Length - 2;

                OrderSqlBase = sql.ToString();
            }
            else
            {
                ByKeySql = null;
                OrderSqlBase = null;
            }
        }

        public static string ByKey() => ByKeySql;

        public static string Where(string condition) => WhereSqlBase + condition;

        public static string Build(string condition = null, string order = null, string column = null, string group = null, string table = null)
        {
            var tableInfo = Metadata<T>.Table;
            var sql = new StringBuilder(256);

            if (String.IsNullOrEmpty(column))
            {
                sql.Append("SELECT * FROM ");
            }
            else
            {
                sql.Append("SELECT ");
                sql.Append(column);
                sql.Append(" FROM ");
            }

            sql.Append(String.IsNullOrEmpty(table) ? tableInfo.Name : table);

            if (!String.IsNullOrEmpty(condition))
            {
                sql.Append(" WHERE ");
                sql.Append(condition);
            }

            if (!String.IsNullOrEmpty(group))
            {
                sql.Append(" GROUP BY ");
                sql.Append(group);
            }

            if (String.IsNullOrEmpty(order))
            {
                if (!String.IsNullOrEmpty(OrderSqlBase))
                {
                    sql.Append(OrderSqlBase);
                }
            }
            else
            {
                sql.Append(" ORDER BY ");
                sql.Append(order);
            }

            return sql.ToString();
        }
    }
}
