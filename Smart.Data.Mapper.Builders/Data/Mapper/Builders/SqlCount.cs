namespace Smart.Data.Mapper.Builders
{
    using System.Text;

    using Smart.Data.Mapper.Builders.Metadata;

    public static class SqlCount<T>
    {
        private static readonly string AllSql;

        private static readonly string ByConditionSqlBase;

        static SqlCount()
        {
            var tableInfo = Metadata<T>.Table;
            var sql = new StringBuilder(256);

            sql.Append("SELECT COUNT(*) FROM ");
            sql.Append(tableInfo.Name);

            AllSql = sql.ToString();

            sql.Append(" WHERE ");

            ByConditionSqlBase = sql.ToString();
        }

        public static string All() => AllSql;

        public static string By(string condition) => ByConditionSqlBase + condition;
    }
}
