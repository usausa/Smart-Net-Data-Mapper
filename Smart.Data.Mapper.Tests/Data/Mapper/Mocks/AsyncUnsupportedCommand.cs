namespace Smart.Data.Mapper.Mocks
{
    using System;
    using System.Data;

    public sealed class AsyncUnsupportedCommand : IDbCommand
    {
        public string CommandText { get; set; }

        public int CommandTimeout { get; set; }

        public CommandType CommandType { get; set; }

        public IDbConnection Connection { get; set; }

        public IDataParameterCollection Parameters => null;

        public IDbTransaction Transaction { get; set; }

        public UpdateRowSource UpdatedRowSource { get; set; }

        public void Dispose()
        {
        }

        public void Cancel()
        {
        }

        public IDbDataParameter CreateParameter() => throw new NotSupportedException();

        public int ExecuteNonQuery() => throw new NotSupportedException();

        public IDataReader ExecuteReader() => throw new NotSupportedException();

        public IDataReader ExecuteReader(CommandBehavior behavior) => throw new NotSupportedException();

        public object ExecuteScalar() => throw new NotSupportedException();

        public void Prepare() => throw new NotSupportedException();
    }
}
