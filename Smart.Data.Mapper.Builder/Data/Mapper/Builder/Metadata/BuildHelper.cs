namespace Smart.Data.Mapper.Builder.Metadata
{
    using System.Text;

    public static class BuildHelper
    {
        public static void BuildKeyCondition(StringBuilder sql, TableInfo tableInfo)
        {
            foreach (var column in tableInfo.KeyColumns)
            {
                sql.Append(column.Name);
                sql.Append(" = ");
                sql.Append("@");
                sql.Append(column.Property.Name);
                sql.Append(" AND ");
            }

            sql.Length = sql.Length - 5;
        }
    }
}
