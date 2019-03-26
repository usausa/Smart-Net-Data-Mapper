namespace Smart.Data.Mapper.Builder
{
    using System.Text;

    using Smart.Data.Mapper.Builder.Metadata;

    public static class Delete<T>
    {
        private static readonly string ByKeySql;

        private static readonly string ByConditionSqlBase;

        static Delete()
        {
            var tableInfo = Metadata<T>.Table;
            var sql = new StringBuilder(256);

            sql.Append("DELETE FROM ");
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
