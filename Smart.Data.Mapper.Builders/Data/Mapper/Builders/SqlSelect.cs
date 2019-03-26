namespace Smart.Data.Mapper.Builders
{
    using System.Text;

    using Smart.Data.Mapper.Builders.Metadata;

    public static class SqlSelect<T>
    {
        private static readonly string ByKeySql;

        private static readonly string ByConditionSqlBase;

        static SqlSelect()
        {
            var tableInfo = Metadata<T>.Table;
            var sql = new StringBuilder(256);

            sql.Append("SELECT * FROM ");
            sql.Append(tableInfo.Name);
            sql.Append(" WHERE ");

            ByConditionSqlBase = sql.ToString();

            BuildHelper.BuildKeyCondition(sql, tableInfo);

            ByKeySql = sql.ToString();
        }

        public static string ByKey() => ByKeySql;

        public static string By(string condition) => ByConditionSqlBase + condition;
    }
}
