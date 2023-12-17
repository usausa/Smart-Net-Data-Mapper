namespace Smart.Data.Mapper;

public sealed class SqlMapperException : Exception
{
    public SqlMapperException()
    {
    }

    public SqlMapperException(string message)
        : base(message)
    {
    }

    public SqlMapperException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
