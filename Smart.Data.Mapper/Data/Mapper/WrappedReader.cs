namespace Smart.Data.Mapper;

using System.Data;
using System.Data.Common;

internal sealed class WrappedReader : DbDataReader
{
    private readonly DbCommand command;

    private readonly DbDataReader reader;

    public override int FieldCount => reader.FieldCount;

    public override object this[int ordinal] => reader[ordinal];

    public override object this[string name] => reader[name];

    public override int Depth => reader.Depth;

    public override bool IsClosed => reader.IsClosed;

    public override int RecordsAffected => reader.RecordsAffected;

    public override bool HasRows => reader.HasRows;

    public WrappedReader(DbCommand command, DbDataReader reader)
    {
        this.command = command;
        this.reader = reader;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            reader.Dispose();
            command.Dispose();
        }
        base.Dispose(disposing);
    }

    public override async ValueTask DisposeAsync()
    {
        await reader.DisposeAsync().ConfigureAwait(false);
        await command.DisposeAsync().ConfigureAwait(false);
        await base.DisposeAsync().ConfigureAwait(false);
    }

    public override void Close() => reader.Close();

    public override Task CloseAsync() => reader.CloseAsync();

    public override DataTable? GetSchemaTable() => reader.GetSchemaTable();

    public override Task<DataTable?> GetSchemaTableAsync(CancellationToken cancellationToken = default) => reader.GetSchemaTableAsync(cancellationToken);

    public override bool NextResult() => reader.NextResult();

    public override Task<bool> NextResultAsync(CancellationToken cancellationToken) => reader.NextResultAsync(cancellationToken);

    public override bool Read() => reader.Read();

    public override Task<bool> ReadAsync(CancellationToken cancellationToken) => reader.ReadAsync(cancellationToken);

    // ReSharper disable once SuspiciousTypeConversion.Global
    public override IEnumerator<object> GetEnumerator() => ((IEnumerable<object>)reader).GetEnumerator();

    public override int GetOrdinal(string name) => reader.GetOrdinal(name);

    public override string GetDataTypeName(int ordinal) => reader.GetDataTypeName(ordinal);

    public override Type GetFieldType(int ordinal) => reader.GetFieldType(ordinal);

    public override string GetName(int ordinal) => reader.GetName(ordinal);

    public override bool IsDBNull(int ordinal) => reader.IsDBNull(ordinal);

    public override Task<bool> IsDBNullAsync(int ordinal, CancellationToken cancellationToken) => reader.IsDBNullAsync(ordinal, cancellationToken);

    public override T GetFieldValue<T>(int ordinal) => reader.GetFieldValue<T>(ordinal);

    public override Task<T> GetFieldValueAsync<T>(int ordinal, CancellationToken cancellationToken) => reader.GetFieldValueAsync<T>(ordinal, cancellationToken);

    public override object GetValue(int ordinal) => reader.GetValue(ordinal);

    public override int GetValues(object[] values) => reader.GetValues(values);

    public override bool GetBoolean(int ordinal) => reader.GetBoolean(ordinal);

    public override byte GetByte(int ordinal) => reader.GetByte(ordinal);

    public override long GetBytes(int ordinal, long dataOffset, byte[]? buffer, int bufferOffset, int length) => reader.GetBytes(ordinal, dataOffset, buffer, bufferOffset, length);

    public override char GetChar(int ordinal) => reader.GetChar(ordinal);

    public override long GetChars(int ordinal, long dataOffset, char[]? buffer, int bufferOffset, int length) => reader.GetChars(ordinal, dataOffset, buffer, bufferOffset, length);

    public override DateTime GetDateTime(int ordinal) => reader.GetDateTime(ordinal);

    public override decimal GetDecimal(int ordinal) => reader.GetDecimal(ordinal);

    public override double GetDouble(int ordinal) => reader.GetDouble(ordinal);

    public override float GetFloat(int ordinal) => reader.GetFloat(ordinal);

    public override Guid GetGuid(int ordinal) => reader.GetGuid(ordinal);

    public override short GetInt16(int ordinal) => reader.GetInt16(ordinal);

    public override int GetInt32(int ordinal) => reader.GetInt32(ordinal);

    public override long GetInt64(int ordinal) => reader.GetInt64(ordinal);

    public override string GetString(int ordinal) => reader.GetString(ordinal);
}
