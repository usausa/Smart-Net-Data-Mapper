namespace Smart.Data.Mapper.Mocks
{
    using System;
    using System.Data;

    public sealed class CommandUnsupportedConnection : IDbConnection
    {
        public string ConnectionString { get; set; }

        public int ConnectionTimeout { get; set; }

        public string Database { get; set; }

        public ConnectionState State { get; private set; }

        public void Dispose()
        {
        }

        public void Open()
        {
            State = ConnectionState.Open;
        }

        public void Close()
        {
            State = ConnectionState.Closed;
        }

        public IDbCommand CreateCommand()
        {
            throw new NotSupportedException();
        }

        public IDbTransaction BeginTransaction() => throw new NotSupportedException();

        public IDbTransaction BeginTransaction(IsolationLevel il) => throw new NotSupportedException();

        public void ChangeDatabase(string databaseName) => throw new NotSupportedException();
    }
}
