namespace Smart.Data.Mapper.Benchmark
{
    using System;
    using System.Linq;

    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Configs;
    using BenchmarkDotNet.Diagnosers;
    using BenchmarkDotNet.Exporters;
    using BenchmarkDotNet.Jobs;
    using BenchmarkDotNet.Running;

    using Smart.Mock.Data;

    public static class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<Benchmark>();
        }
    }

    public class BenchmarkConfig : ManualConfig
    {
        public BenchmarkConfig()
        {
            Add(MarkdownExporter.Default, MarkdownExporter.GitHub);
            Add(MemoryDiagnoser.Default);
            Add(Job.LongRun);
        }
    }

    [Config(typeof(BenchmarkConfig))]
    public class Benchmark
    {
        private MockDbConnection mockExecute;

        private MockDbConnection mockExecuteScalar;

        private MockDbConnection mockQuery;

        private MockDbConnection mockQueryFirst;

        [IterationSetup]
        public void IterationSetup()
        {
            mockExecute = new MockDbConnection();
            mockExecute.SetupCommand(cmd => cmd.SetupResult(1));

            mockExecuteScalar = new MockDbConnection();
            mockExecuteScalar.SetupCommand(cmd => cmd.SetupResult(1L));

            mockQuery = new MockDbConnection();
            mockQuery.SetupCommand(cmd => cmd.SetupResult(new MockDataReader(
                new[]
                {
                    new MockColumn(typeof(long), "Id"),
                    new MockColumn(typeof(string), "Name")
                },
                Enumerable.Range(1, 100).Select(x => new object[]
                {
                    (long)x,
                    "test"
                }).ToList())));

            mockQueryFirst = new MockDbConnection();
            mockQueryFirst.SetupCommand(cmd => cmd.SetupResult(new MockDataReader(
                new[]
                {
                    new MockColumn(typeof(long), "Id"),
                    new MockColumn(typeof(string), "Name"),
                    new MockColumn(typeof(int), "Amount"),
                    new MockColumn(typeof(int), "Qty"),
                    new MockColumn(typeof(bool), "Flag1"),
                    new MockColumn(typeof(bool), "Flag2"),
                    new MockColumn(typeof(DateTimeOffset), "CreatedAt"),
                    new MockColumn(typeof(string), "CreatedBy"),
                    new MockColumn(typeof(DateTimeOffset?), "UpdatedAt"),
                    new MockColumn(typeof(string), "UpdatedBy"),
                },
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
                }).ToList())));
        }

        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        private const string ExecuteSql =
            "INSERT INTO Table (Id, Data) VALUES (@Id, @Data)";

        [Benchmark]
        public void DapperExecute()
        {
            Dapper.SqlMapper.Execute(mockExecute, ExecuteSql, new { Id = 1, Data = "test" });
        }

        [Benchmark]
        public void SmartExecute()
        {
            mockExecute.Execute(ExecuteSql, new { Id = 1, Data = "test" });
        }

        private const string ExecuteWithParameter10Sql =
            "INSERT INTO Table (Id, Name, Amount, Qty, Flag1, Flag2, DateTimeOffset, CreatedBy, UpdatedAt, UpdatedBy) " +
            "VALUES (@Id, @Name, @Amount, @Qty, @Flag1, @Flag2, @DateTimeOffset, @CreatedBy, @UpdatedAt, @UpdatedBy)";

        [Benchmark]
        public void DapperExecuteWithParameter10()
        {
            Dapper.SqlMapper.Execute(mockExecute, ExecuteWithParameter10Sql, new LargeData());
        }

        [Benchmark]
        public void SmartExecuteWithParameter10()
        {
            mockExecute.Execute(ExecuteWithParameter10Sql, new LargeData());
        }

        [Benchmark]
        public void DapperExecuteWithOverParameter()
        {
            // [MEMO] Dapper optimize parameters
            Dapper.SqlMapper.Execute(mockExecute, ExecuteSql, new LargeData());
        }

        [Benchmark]
        public void SmartExecuteWithOverParameter()
        {
            mockExecute.Execute(ExecuteSql, new LargeData());
        }

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        private const string ExecuteScalarSql =
            "SELECT COUNT(*) FROM Table";

        [Benchmark]
        public long DapperExecuteScalar()
        {
            return Dapper.SqlMapper.ExecuteScalar<long>(mockExecuteScalar, ExecuteScalarSql);
        }

        [Benchmark]
        public long SmartExecuteScalar()
        {
            return mockExecuteScalar.ExecuteScalar<long>(ExecuteScalarSql);
        }

        [Benchmark]
        public long DapperExecuteScalarWithConvert()
        {
            return Dapper.SqlMapper.ExecuteScalar<int>(mockExecuteScalar, ExecuteScalarSql);
        }

        [Benchmark]
        public long SmartExecuteScalarWithConvert()
        {
            return mockExecuteScalar.ExecuteScalar<int>(ExecuteScalarSql);
        }

        //--------------------------------------------------------------------------------
        // Query
        //--------------------------------------------------------------------------------

        private const string QuerySql =
            "SELECT * FROM Data ORDER BY Id";

        [Benchmark]
        public void DapperQuery100()
        {
            foreach (var dummy in Dapper.SqlMapper.Query<Data>(mockQuery, QuerySql, buffered: false))
            {
            }
        }

        [Benchmark]
        public void SmartQuery100()
        {
            foreach (var dummy in mockQuery.Query<Data>(QuerySql))
            {
            }
        }

        //--------------------------------------------------------------------------------
        // Query
        //--------------------------------------------------------------------------------

        private const string QueryFirstSql =
            "SELECT * FROM LargeData WHERE Id = 1";

        [Benchmark]
        public LargeData DapperQueryFirst()
        {
            return Dapper.SqlMapper.QueryFirstOrDefault<LargeData>(mockQueryFirst, QueryFirstSql);
        }

        [Benchmark]
        public LargeData SmartQueryFirst()
        {
            return mockQueryFirst.QueryFirstOrDefault<LargeData>(QueryFirstSql);
        }
    }

    public class Data
    {
        public long Id { get; set; }

        public string Name { get; set; }
    }

    public class LargeData
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public int Amount { get; set; }

        public int Qty { get; set; }

        public bool Flag1 { get; set; }

        public bool Flag2 { get; set; }

        public DateTimeOffset CreatedAt { get; set; }

        public string CreatedBy { get; set; }

        public DateTimeOffset? UpdatedAt { get; set; }

        public string UpdatedBy { get; set; }
    }
}
