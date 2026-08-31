namespace Smart.Data.Mapper.Builders.Metadata;

using System.Text;

public static class BuildHelper
{
    public static void BuildKeyCondition(StringBuilder sql, TableMetadata tableInfo)
    {
        foreach (var column in tableInfo.KeyColumns)
        {
            sql.Append(column.Name);
            sql.Append(" = ");
            sql.Append('@');
            sql.Append(column.Property.Name);
            sql.Append(" AND ");
        }

        sql.Length -= 5;
    }
}
