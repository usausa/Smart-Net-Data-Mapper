namespace Smart.Data.Mapper.Benchmark;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Exporters.Csv;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

using Smart.Mock.Data;

public static class Program
{
    public static void Main()
    {
        BenchmarkRunner.Run<DataMapperBenchmark>();
    }
}

public sealed class BenchmarkConfig : ManualConfig
{
    public BenchmarkConfig()
    {
        AddExporter(MarkdownExporter.GitHub);
        AddExporter(CsvExporter.Default);
        //AddExporter(CsvMeasurementsExporter.Default);
        //AddExporter(RPlotExporter.Default);
        AddColumn(
            StatisticColumn.Mean,
            StatisticColumn.Min,
            StatisticColumn.Max,
            StatisticColumn.P90,
            StatisticColumn.Error,
            StatisticColumn.StdDev);
        AddDiagnoser(MemoryDiagnoser.Default);
        AddJob(Job.MediumRun);
    }
}

[Config(typeof(BenchmarkConfig))]
#pragma warning disable CA1001
public class DataMapperBenchmark
{
    private const int N = 1000;

    private MockRepeatDbConnection mockExecute = default!;

    private MockRepeatDbConnection mockExecuteScalar = default!;

    private MockRepeatDbConnection mockQuery = default!;

    private MockRepeatDbConnection mockQueryFirst = default!;

#pragma warning disable CA2000
    [GlobalSetup]
    public void Setup()
    {
        mockExecute = new MockRepeatDbConnection(1);

        mockExecuteScalar = new MockRepeatDbConnection(1L);

        mockQuery = new MockRepeatDbConnection(new MockDataReader(
            [
                new MockColumn(typeof(long), "Id"),
                new MockColumn(typeof(string), "Name")
            ],
            Enumerable.Range(1, 100).Select(x => new object[]
            {
                (long)x,
                "test"
            })));

        mockQueryFirst = new MockRepeatDbConnection(new MockDataReader(
            [
                new MockColumn(typeof(long), "Id"),
                new MockColumn(typeof(string), "Name"),
                new MockColumn(typeof(int), "Amount"),
                new MockColumn(typeof(int), "Qty"),
                new MockColumn(typeof(bool), "Flag1"),
                new MockColumn(typeof(bool), "Flag2"),
                new MockColumn(typeof(DateTimeOffset), "CreatedAt"),
                new MockColumn(typeof(string), "CreatedBy"),
                new MockColumn(typeof(DateTimeOffset?), "UpdatedAt"),
                new MockColumn(typeof(string), "UpdatedBy")
            ],
            Enumerable.Range(1, 1).Select(x => new object[]
            {
                (long)x,
                "test",
                1,
                2,
                true,
                false,
                DateTimeOffset.Now,
                "user",
                DBNull.Value,
                DBNull.Value
            })));
    }
#pragma warning restore CA2000

    [GlobalCleanup]
    public void Cleanup()
    {
        mockExecute.Dispose();
        mockExecuteScalar.Dispose();
        mockQuery.Dispose();
        mockQueryFirst.Dispose();
    }

    //--------------------------------------------------------------------------------
    // Execute
    //--------------------------------------------------------------------------------

    private const string ExecuteSql =
        "INSERT INTO Table (Id, Data) VALUES (@Id, @Data)";

    [Benchmark(OperationsPerInvoke = N)]
    public void DapperExecute()
    {
        for (var i = 0; i < N; i++)
        {
            Dapper.SqlMapper.Execute(mockExecute, ExecuteSql, new { Id = 1, Data = "test" });
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void SmartExecute()
    {
        for (var i = 0; i < N; i++)
        {
            mockExecute.Execute(ExecuteSql, new { Id = 1, Data = "test" });
        }
    }

    private const string ExecuteWithParameter10Sql =
        "INSERT INTO Table (Id, Name, Amount, Qty, Flag1, Flag2, DateTimeOffset, CreatedBy, UpdatedAt, UpdatedBy) " +
        "VALUES (@Id, @Name, @Amount, @Qty, @Flag1, @Flag2, @DateTimeOffset, @CreatedBy, @UpdatedAt, @UpdatedBy)";

    [Benchmark(OperationsPerInvoke = N)]
    public void DapperExecuteWithParameter10()
    {
        for (var i = 0; i < N; i++)
        {
            Dapper.SqlMapper.Execute(mockExecute, ExecuteWithParameter10Sql, new LargeDataEntity());
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void SmartExecuteWithParameter10()
    {
        for (var i = 0; i < N; i++)
        {
            mockExecute.Execute(ExecuteWithParameter10Sql, new LargeDataEntity());
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void DapperExecuteWithOverParameter()
    {
        for (var i = 0; i < N; i++)
        {
            // [MEMO] Dapper optimize parameters
            Dapper.SqlMapper.Execute(mockExecute, ExecuteSql, new LargeDataEntity());
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void SmartExecuteWithOverParameter()
    {
        for (var i = 0; i < N; i++)
        {
            mockExecute.Execute(ExecuteSql, new LargeDataEntity());
        }
    }

    //--------------------------------------------------------------------------------
    // ExecuteScalar
    //--------------------------------------------------------------------------------

    private const string ExecuteScalarSql =
        "SELECT COUNT(*) FROM Table";

    [Benchmark(OperationsPerInvoke = N)]
    public long DapperExecuteScalar()
    {
        long ret = default;
        for (var i = 0; i < N; i++)
        {
            ret = Dapper.SqlMapper.ExecuteScalar<long>(mockExecuteScalar, ExecuteScalarSql);
        }
        return ret;
    }

    [Benchmark(OperationsPerInvoke = N)]
    public long SmartExecuteScalar()
    {
        long ret = default;
        for (var i = 0; i < N; i++)
        {
            ret = mockExecuteScalar.ExecuteScalar<long>(ExecuteScalarSql);
        }
        return ret;
    }

    [Benchmark(OperationsPerInvoke = N)]
    public long DapperExecuteScalarWithConvert()
    {
        long ret = default;
        for (var i = 0; i < N; i++)
        {
            ret = Dapper.SqlMapper.ExecuteScalar<int>(mockExecuteScalar, ExecuteScalarSql);
        }
        return ret;
    }

    [Benchmark(OperationsPerInvoke = N)]
    public long SmartExecuteScalarWithConvert()
    {
        long ret = default;
        for (var i = 0; i < N; i++)
        {
            ret = mockExecuteScalar.ExecuteScalar<int>(ExecuteScalarSql);
        }
        return ret;
    }

    //--------------------------------------------------------------------------------
    // Query
    //--------------------------------------------------------------------------------

    private const string QuerySql =
        "SELECT * FROM Data ORDER BY Id";

    [Benchmark(OperationsPerInvoke = N)]
    public void DapperQuery100()
    {
        for (var i = 0; i < N; i++)
        {
            using var en = Dapper.SqlMapper.Query<DataEntity>(mockQuery, QuerySql, false).GetEnumerator();
            while (en.MoveNext())
            {
            }
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void SmartQuery100()
    {
        for (var i = 0; i < N; i++)
        {
            using var en = mockQuery.Query<DataEntity>(QuerySql).GetEnumerator();
            while (en.MoveNext())
            {
            }
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void DapperQuery100Bufferd()
    {
        for (var i = 0; i < N; i++)
        {
            Dapper.SqlMapper.Query<DataEntity>(mockQuery, QuerySql, true);
        }
    }

    [Benchmark(OperationsPerInvoke = N)]
    public void SmartQuery100Bufferd()
    {
        for (var i = 0; i < N; i++)
        {
            mockQuery.QueryList<DataEntity>(QuerySql);
        }
    }

    //--------------------------------------------------------------------------------
    // Query
    //--------------------------------------------------------------------------------

    private const string QueryFirstSql =
        "SELECT * FROM LargeData WHERE Id = 1";

    [Benchmark(OperationsPerInvoke = N)]
    public LargeDataEntity? DapperQueryFirst()
    {
        LargeDataEntity? ret = default;
        for (var i = 0; i < N; i++)
        {
            ret = Dapper.SqlMapper.QueryFirstOrDefault<LargeDataEntity>(mockQueryFirst, QueryFirstSql);
        }
        return ret;
    }

    [Benchmark(OperationsPerInvoke = N)]
    public LargeDataEntity? SmartQueryFirst()
    {
        LargeDataEntity? ret = default;
        for (var i = 0; i < N; i++)
        {
            ret = mockQueryFirst.QueryFirstOrDefault<LargeDataEntity>(QueryFirstSql);
        }
        return ret;
    }
}

public sealed class DataEntity
{
    public long Id { get; set; }

    public string? Name { get; set; }
}

public sealed class LargeDataEntity
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public int Amount { get; set; }

    public int Qty { get; set; }

    public bool Flag1 { get; set; }

    public bool Flag2 { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public string? CreatedBy { get; set; }

    public DateTimeOffset? UpdatedAt { get; set; }

    public string? UpdatedBy { get; set; }
}
#pragma warning restore CA1001
