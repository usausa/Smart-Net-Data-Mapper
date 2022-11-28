# Smart.Data.Mapper .NET - micro-orm library for .NET

[![NuGet Badge](https://buildstats.info/nuget/Usa.Smart.Data.Mapper)](https://www.nuget.org/packages/Usa.Smart.Data.Mapper/)

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
public class TestProcParameter
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

Job=MediumRun  Jit=RyuJit  Platform=X64  
IterationCount=15  LaunchCount=2  WarmupCount=10  

```
|                         Method |        Mean |     Error |    StdDev |      Median |         Min |         Max |         P90 |   Gen0 |   Gen1 | Allocated |
|------------------------------- |------------:|----------:|----------:|------------:|------------:|------------:|------------:|-------:|-------:|----------:|
|                  DapperExecute |   179.40 ns |  1.030 ns |  1.509 ns |   179.39 ns |   175.50 ns |   182.66 ns |   181.11 ns | 0.0271 |      - |     456 B |
|                   SmartExecute |   102.57 ns |  1.219 ns |  1.787 ns |   101.63 ns |   100.44 ns |   105.11 ns |   104.70 ns | 0.0220 |      - |     368 B |
|   DapperExecuteWithParameter10 |   445.69 ns |  3.070 ns |  4.500 ns |   445.43 ns |   437.88 ns |   456.54 ns |   452.56 ns | 0.0747 |      - |    1256 B |
|    SmartExecuteWithParameter10 |   296.94 ns |  4.504 ns |  6.741 ns |   297.04 ns |   283.99 ns |   305.97 ns |   304.45 ns | 0.0747 |      - |    1256 B |
| DapperExecuteWithOverParameter |   163.24 ns |  1.014 ns |  1.487 ns |   162.92 ns |   160.38 ns |   166.62 ns |   165.35 ns | 0.0281 |      - |     472 B |
|  SmartExecuteWithOverParameter |   301.11 ns |  2.100 ns |  3.078 ns |   301.29 ns |   292.85 ns |   307.02 ns |   305.21 ns | 0.0747 |      - |    1256 B |
|            DapperExecuteScalar |    61.81 ns |  0.410 ns |  0.601 ns |    61.69 ns |    60.75 ns |    63.28 ns |    62.67 ns | 0.0085 |      - |     144 B |
|             SmartExecuteScalar |    50.80 ns |  0.366 ns |  0.536 ns |    50.72 ns |    49.94 ns |    52.03 ns |    51.49 ns | 0.0086 |      - |     144 B |
| DapperExecuteScalarWithConvert |    82.09 ns |  0.385 ns |  0.564 ns |    82.04 ns |    80.59 ns |    82.93 ns |    82.83 ns | 0.0100 |      - |     168 B |
|  SmartExecuteScalarWithConvert |    76.51 ns |  0.514 ns |  0.753 ns |    76.65 ns |    74.79 ns |    77.70 ns |    77.60 ns | 0.0100 |      - |     168 B |
|                 DapperQuery100 | 2,947.10 ns | 22.614 ns | 33.147 ns | 2,947.49 ns | 2,889.19 ns | 3,017.24 ns | 2,986.49 ns | 0.3516 | 0.0039 |    5896 B |
|                  SmartQuery100 | 1,626.52 ns | 10.269 ns | 15.370 ns | 1,627.42 ns | 1,591.71 ns | 1,651.99 ns | 1,646.23 ns | 0.2090 |      - |    3520 B |
|          DapperQuery100Bufferd | 2,494.62 ns | 12.598 ns | 18.466 ns | 2,493.23 ns | 2,456.25 ns | 2,528.42 ns | 2,516.87 ns | 0.3477 | 0.0039 |    5856 B |
|           SmartQuery100Bufferd | 1,665.39 ns |  6.149 ns |  8.619 ns | 1,665.98 ns | 1,642.77 ns | 1,680.88 ns | 1,674.15 ns | 0.3301 | 0.0039 |    5536 B |
|               DapperQueryFirst |   255.55 ns |  1.107 ns |  1.657 ns |   255.54 ns |   252.83 ns |   259.20 ns |   257.60 ns | 0.0200 |      - |     336 B |
|                SmartQueryFirst |   306.22 ns |  4.470 ns |  6.552 ns |   310.07 ns |   297.42 ns |   315.23 ns |   313.15 ns | 0.0146 |      - |     248 B |

Not so late( ˙ω˙)?
