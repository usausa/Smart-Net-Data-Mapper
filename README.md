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

``` ini

BenchmarkDotNet=v0.13.2, OS=Windows 11 (10.0.22621.819)
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=7.0.100
  [Host]    : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2
  MediumRun : .NET 7.0.0 (7.0.22.51805), X64 RyuJIT AVX2

Job=MediumRun  IterationCount=15  LaunchCount=2  
WarmupCount=10  

```
|                         Method |        Mean |     Error |    StdDev |      Median |         Min |         Max |         P90 |   Gen0 |   Gen1 | Allocated |
|------------------------------- |------------:|----------:|----------:|------------:|------------:|------------:|------------:|-------:|-------:|----------:|
|                  DapperExecute |   184.26 ns |  3.176 ns |  4.452 ns |   182.41 ns |   178.70 ns |   191.21 ns |   189.87 ns | 0.0271 |      - |     456 B |
|                   SmartExecute |   104.52 ns |  0.839 ns |  1.203 ns |   104.79 ns |   102.71 ns |   106.78 ns |   106.13 ns | 0.0220 |      - |     368 B |
|   DapperExecuteWithParameter10 |   446.17 ns |  2.022 ns |  2.963 ns |   446.59 ns |   439.95 ns |   451.81 ns |   449.78 ns | 0.0747 |      - |    1256 B |
|    SmartExecuteWithParameter10 |   282.75 ns |  2.261 ns |  3.385 ns |   282.79 ns |   275.52 ns |   289.75 ns |   287.01 ns | 0.0747 |      - |    1256 B |
| DapperExecuteWithOverParameter |   164.08 ns |  1.723 ns |  2.415 ns |   164.64 ns |   160.00 ns |   168.18 ns |   167.26 ns | 0.0281 |      - |     472 B |
|  SmartExecuteWithOverParameter |   290.39 ns |  4.898 ns |  7.331 ns |   290.72 ns |   279.06 ns |   303.17 ns |   299.20 ns | 0.0747 |      - |    1256 B |
|            DapperExecuteScalar |    58.85 ns |  0.201 ns |  0.301 ns |    58.86 ns |    58.34 ns |    59.45 ns |    59.22 ns | 0.0086 |      - |     144 B |
|             SmartExecuteScalar |    52.41 ns |  1.257 ns |  1.762 ns |    52.36 ns |    50.27 ns |    55.10 ns |    54.50 ns | 0.0086 |      - |     144 B |
| DapperExecuteScalarWithConvert |    88.37 ns |  1.996 ns |  2.926 ns |    86.94 ns |    84.98 ns |    94.95 ns |    92.06 ns | 0.0100 |      - |     168 B |
|  SmartExecuteScalarWithConvert |    82.55 ns |  3.658 ns |  5.361 ns |    86.63 ns |    76.51 ns |    88.34 ns |    88.12 ns | 0.0100 |      - |     168 B |
|                 DapperQuery100 | 2,917.63 ns | 15.512 ns | 22.737 ns | 2,917.30 ns | 2,880.38 ns | 2,965.22 ns | 2,949.00 ns | 0.3516 | 0.0039 |    5896 B |
|                  SmartQuery100 | 1,601.73 ns |  9.355 ns | 14.002 ns | 1,599.81 ns | 1,580.57 ns | 1,637.07 ns | 1,621.92 ns | 0.2090 |      - |    3520 B |
|          DapperQuery100Bufferd | 2,498.05 ns | 11.891 ns | 17.799 ns | 2,493.76 ns | 2,451.89 ns | 2,527.10 ns | 2,524.58 ns | 0.3477 | 0.0039 |    5856 B |
|           SmartQuery100Bufferd | 1,718.58 ns | 15.172 ns | 22.709 ns | 1,715.81 ns | 1,675.41 ns | 1,765.25 ns | 1,750.48 ns | 0.3301 | 0.0039 |    5536 B |
|               DapperQueryFirst |   254.31 ns |  1.509 ns |  2.259 ns |   254.65 ns |   249.74 ns |   257.06 ns |   256.85 ns | 0.0200 |      - |     336 B |
|                SmartQueryFirst |   292.17 ns |  1.981 ns |  2.903 ns |   293.33 ns |   285.74 ns |   296.98 ns |   295.40 ns | 0.0146 |      - |     248 B |

Not so late( ˙ω˙)?
