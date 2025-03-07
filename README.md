# Smart.Data.Mapper .NET - micro-orm library for .NET

[![NuGet](https://img.shields.io/nuget/v/Usa.Smart.Data.Mapper.svg)](https://www.nuget.org/packages/Usa.Smart.Data.Mapper)

## What is this?

* Smart.Data.Mapper .NET is simplified micro-orm library, degradation version of Dapper.
* ***Main use is for environments that don't support code generation.***
* Also support for environments that support code generation.

### Usage example

```csharp
// Execute
var effect = con.Execute("INSERT INTO Data (Id, Name, Code) VALUES (@Id, @Name, @Code)", new { Id = 1, Name = "test", Code = "A" });

// Execute scalar
var count = con.ExecuteScalar<long>("SELECT COUNT(*) FROM Data WHERE Code = @Code", new { Code = "A" });

// Query list
foreach (var entity in con.Query<Data>("SELECT * FROM Data ORDER BY Id"))
{
}

// Query one
var entity = con.QueryFirstOrDefault<Data>("SELECT COUNT(*) FROM Data WHERE Id = @Id", new { Id = 1 });

// Execute procedure
public sealed class TestProcParameter
{
    public int InParam { get; set; }

    [Direction(ParameterDirection.Output)]
    public long OutParam { get; set; }
}

var parameter = new TestProcParameter { InParam = 1 };
con.Execute("TestProc", parameter, commandType: CommandType.StoredProcedure);
var result = parameter.OutParam;
```

## NuGet

| Package |
|-|
| [![NuGet Badge](https://buildstats.info/nuget/Usa.Smart.Data.Mapper)](https://www.nuget.org/packages/Usa.Smart.Data.Mapper/) |
| [![NuGet Badge](https://buildstats.info/nuget/Usa.Smart.Data.Mapper.Builders)](https://www.nuget.org/packages/Usa.Smart.Data.Mapper.Builders/) |

## Functions

### Execute

```csharp
int Execute(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

int Execute(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

async ValueTask<int> ExecuteAsync(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = default)

Task<int> ExecuteAsync(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = default)
```

### ExecuteScalar

```csharp
T ExecuteScalar<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

T ExecuteScalar<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

async ValueTask<T> ExecuteScalarAsync<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = default)

Task<T> ExecuteScalarAsync<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = default)
```

### ExecuteReader

```csharp
IDataReader ExecuteReader(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default)

IDataReader ExecuteReader(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default)

async ValueTask<DbDataReader> ExecuteReaderAsync(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken token = default)

Task<DbDataReader> ExecuteReaderAsync(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken token = default)
```

### Query

```csharp
IEnumerable<T> Query<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

IEnumerable<T> Query<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

async IAsyncEnumerable<T> QueryAsync<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)

IAsyncEnumerable<T> QueryAsync<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
```

### QueryList

```csharp
List<T> QueryList<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

List<T> QueryList<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

async ValueTask<List<T>> QueryListAsync<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)

Task<List<T>> QueryListAsync<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
```

### QueryFirstOrDefault

```csharp
T QueryFirstOrDefault<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

T QueryFirstOrDefault<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

async ValueTask<T> QueryFirstOrDefaultAsync<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = default)

Task<T> QueryFirstOrDefaultAsync<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = default)
```

## Parameter/Result

### Mapping option

* Pascal and snake case support by default
* NameAttribute support
* IgnoreAttribute support

### Parameter option

* DirectionAttribute support
* DbTypeAttribute support
* SizeAttribute support
* IgnoreAttribute support

### DynamicParameter

* Like Dapper

### TypeHandler

* Like Dapper

## SqlMapperConfig

### Propeties

```csharp
// Reflection api abstraction
IDelegateFactory DelegateFactory { get; set; }

// Type converter
IObjectConverter Converter { get; set; }

// Mapping property selector
IPropertySelector PropertySelector { get; set; }
```

### Methods

```csharp
// Reset IParameterBuilderFactory's
SqlMapperConfig ResetParameterBuilderFactories()

// Supports DynamicParameter, IDictionary<string, object> and object by default
SqlMapperConfig ConfigureBuilderFactory(Action<IList<IParameterBuilderFactory>> action)

// Reset IResultMapperFactory's
SqlMapperConfig ResetResultMappers()

// Supports object by default
SqlMapperConfig ConfigureMapperFactory(Action<IList<IResultMapperFactory>> action)

// Reset type map
SqlMapperConfig ResetTypeMap()

// Configure type map
SqlMapperConfig ConfigureTypeMap(Action<IDictionary<Type, DbType>> action)

// Reset type handlers
SqlMapperConfig ResetTypeHandlers()

// Configure type handlers
SqlMapperConfig ConfigureTypeHandlers(Action<IDictionary<Type, ITypeHandler>> action)
```

## TODO

* Multiple query support ?

## Benchmark(for reference purpose only)

Benchmark result on .NET Core 7 with Code generation mode.

```
BenchmarkDotNet v0.14.0, Windows 11 (10.0.26100.3194)
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK 9.0.200
  [Host]    : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
  MediumRun : .NET 9.0.2 (9.0.225.6610), X64 RyuJIT AVX2
```
| Method                         | Mean        | Error     | StdDev    | Min         | Max         | P90         | Gen0   | Gen1   | Allocated |
|------------------------------- |------------:|----------:|----------:|------------:|------------:|------------:|-------:|-------:|----------:|
| DapperExecute                  |   212.58 ns |  4.694 ns |  7.026 ns |   202.33 ns |   229.03 ns |   222.42 ns | 0.0271 |      - |     456 B |
| SmartExecute                   |    80.62 ns |  1.592 ns |  2.283 ns |    77.19 ns |    85.88 ns |    83.68 ns | 0.0220 |      - |     368 B |
| DapperExecuteWithParameter10   |   483.89 ns |  9.340 ns | 13.093 ns |   466.67 ns |   518.24 ns |   502.96 ns | 0.0742 |      - |    1256 B |
| SmartExecuteWithParameter10    |   264.96 ns |  4.597 ns |  6.593 ns |   253.83 ns |   278.68 ns |   273.28 ns | 0.0747 |      - |    1256 B |
| DapperExecuteWithOverParameter |   207.20 ns | 12.270 ns | 18.365 ns |   181.00 ns |   238.40 ns |   231.96 ns | 0.0281 |      - |     472 B |
| SmartExecuteWithOverParameter  |   260.22 ns |  5.732 ns |  8.580 ns |   243.94 ns |   280.53 ns |   271.56 ns | 0.0747 |      - |    1256 B |
| DapperExecuteScalar            |    93.23 ns |  2.778 ns |  3.984 ns |    88.68 ns |   102.58 ns |    99.87 ns | 0.0085 |      - |     144 B |
| SmartExecuteScalar             |    43.67 ns |  0.866 ns |  1.214 ns |    42.35 ns |    46.85 ns |    45.33 ns | 0.0086 |      - |     144 B |
| DapperExecuteScalarWithConvert |   106.98 ns |  1.846 ns |  2.648 ns |   100.81 ns |   111.66 ns |   110.01 ns | 0.0085 |      - |     144 B |
| SmartExecuteScalarWithConvert  |    68.63 ns |  1.598 ns |  2.392 ns |    65.01 ns |    72.48 ns |    72.33 ns | 0.0100 |      - |     168 B |
| DapperQuery100                 | 2,672.12 ns | 35.815 ns | 53.606 ns | 2,564.74 ns | 2,796.08 ns | 2,750.02 ns | 0.3477 | 0.0039 |    5880 B |
| SmartQuery100                  | 1,192.38 ns | 29.696 ns | 44.447 ns | 1,132.81 ns | 1,289.95 ns | 1,255.94 ns | 0.2090 |      - |    3520 B |
| DapperQuery100Bufferd          | 2,480.81 ns | 29.533 ns | 44.203 ns | 2,391.34 ns | 2,577.36 ns | 2,525.80 ns | 0.3477 | 0.0039 |    5840 B |
| SmartQuery100Bufferd           | 1,143.32 ns | 20.661 ns | 29.631 ns | 1,078.75 ns | 1,216.55 ns | 1,171.49 ns | 0.3301 | 0.0039 |    5536 B |
| DapperQueryFirst               |   270.77 ns |  2.722 ns |  3.990 ns |   262.91 ns |   277.72 ns |   275.80 ns | 0.0200 |      - |     336 B |
| SmartQueryFirst                |   192.03 ns |  2.772 ns |  4.149 ns |   186.47 ns |   201.72 ns |   196.45 ns | 0.0146 |      - |     248 B |

Not so late( ˙ω˙)?
