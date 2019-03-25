namespace Smart.Data.Mapper.Mocks
{
    using System;
    using System.Data;

    public sealed class AsyncUnsupportedConnection : IDbConnection
    {
        private readonly bool commandSupport;

        private ConnectionState state;

        public string ConnectionString { get; set; }

        public int ConnectionTimeout { get; set; }

        public string Database { get; set; }

        public ConnectionState State => state;

        public AsyncUnsupportedConnection(bool commandSupport = false)
        {
            this.commandSupport = commandSupport;
        }

        public void Dispose()
        {
        }

        public void Open()
        {
            state = ConnectionState.Open;
        }

        public void Close()
        {
            state = ConnectionState.Closed;
        }

        public IDbCommand CreateCommand()
        {
            return commandSupport ? (IDbCommand)new AsyncSupportedCommand() : new AsyncUnsupportedCommand();
        }

        public IDbTransaction BeginTransaction() => throw new NotSupportedException();

        public IDbTransaction BeginTransaction(IsolationLevel il) => throw new NotSupportedException();

        public void ChangeDatabase(string databaseName) => throw new NotSupportedException();
    }
}
