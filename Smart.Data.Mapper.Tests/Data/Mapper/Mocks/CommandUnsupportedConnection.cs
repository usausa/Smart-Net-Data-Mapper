namespace Smart.Data.Mapper.Mocks
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics.CodeAnalysis;

    public sealed class CommandUnsupportedConnection : DbConnection
    {
        private ConnectionState state;

        [AllowNull]
        public override string ConnectionString { get; set; }

        public override string Database => string.Empty;

        // ReSharper disable once ConvertToAutoPropertyWithPrivateSetter
        public override ConnectionState State => state;

        public override string DataSource => string.Empty;

        public override string ServerVersion => string.Empty;

        public override void Open()
        {
            state = ConnectionState.Open;
        }

        public override void Close()
        {
            state = ConnectionState.Closed;
        }

        protected override DbCommand CreateDbCommand() => throw new NotSupportedException();

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) => throw new NotSupportedException();

        public override void ChangeDatabase(string databaseName) => throw new NotSupportedException();
    }
}
