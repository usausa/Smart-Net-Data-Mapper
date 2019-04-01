namespace Smart.Data.Mapper.Builders
{
    using System.Text;

    using Smart.Data.Mapper.Builders.Metadata;

    public static class SqlDelete<T>
    {
        private static readonly string ByKeySql;

        private static readonly string WhereSqlBase;

        static SqlDelete()
        {
            var tableInfo = TableInfo<T>.Instance;
            var sql = new StringBuilder(256);

            sql.Append("DELETE FROM ");
            sql.Append(tableInfo.Name);
            sql.Append(" WHERE ");

            WhereSqlBase = sql.ToString();

            if (tableInfo.KeyColumns.Count > 0)
            {
                BuildHelper.BuildKeyCondition(sql, tableInfo);

                ByKeySql = sql.ToString();
            }
            else
            {
                ByKeySql = null;
            }
        }

        public static string ByKey() => ByKeySql;

        public static string Where(string condition) => WhereSqlBase + condition;
    }
}
