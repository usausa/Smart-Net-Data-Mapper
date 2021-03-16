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

Benchmark result on .NET Core 5 with Code generation mode.

``` ini
BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
.NET Core SDK=5.0.201
  [Host]    : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT
  MediumRun : .NET Core 5.0.4 (CoreCLR 5.0.421.11614, CoreFX 5.0.421.11614), X64 RyuJIT

Job=MediumRun  IterationCount=15  LaunchCount=2  
WarmupCount=10  
```
|                         Method |        Mean |     Error |    StdDev |      Median |         Min |         Max |         P90 |  Gen 0 |  Gen 1 | Gen 2 | Allocated |
|------------------------------- |------------:|----------:|----------:|------------:|------------:|------------:|------------:|-------:|-------:|------:|----------:|
|                  DapperExecute |   183.50 ns |  1.111 ns |  1.558 ns |   183.16 ns |   181.80 ns |   186.78 ns |   185.58 ns | 0.0271 |      - |     - |     456 B |
|                   SmartExecute |   102.43 ns |  0.335 ns |  0.480 ns |   102.29 ns |   101.86 ns |   103.89 ns |   103.10 ns | 0.0220 |      - |     - |     368 B |
|   DapperExecuteWithParameter10 |   431.16 ns |  2.567 ns |  3.843 ns |   432.18 ns |   423.95 ns |   437.75 ns |   435.92 ns | 0.0747 |      - |     - |    1256 B |
|    SmartExecuteWithParameter10 |   283.14 ns |  2.540 ns |  3.802 ns |   283.59 ns |   277.31 ns |   289.47 ns |   288.36 ns | 0.0747 |      - |     - |    1256 B |
| DapperExecuteWithOverParameter |   162.68 ns |  0.830 ns |  1.243 ns |   162.77 ns |   159.98 ns |   165.30 ns |   163.84 ns | 0.0281 |      - |     - |     472 B |
|  SmartExecuteWithOverParameter |   282.66 ns |  2.241 ns |  3.284 ns |   281.58 ns |   278.07 ns |   286.83 ns |   286.36 ns | 0.0747 |      - |     - |    1256 B |
|            DapperExecuteScalar |    62.41 ns |  0.379 ns |  0.555 ns |    62.58 ns |    61.73 ns |    63.29 ns |    63.11 ns | 0.0085 |      - |     - |     144 B |
|             SmartExecuteScalar |    52.70 ns |  0.223 ns |  0.327 ns |    52.78 ns |    52.10 ns |    53.31 ns |    53.08 ns | 0.0086 |      - |     - |     144 B |
| DapperExecuteScalarWithConvert |   113.03 ns |  0.921 ns |  1.321 ns |   112.69 ns |   111.50 ns |   115.94 ns |   114.50 ns | 0.0100 |      - |     - |     168 B |
|  SmartExecuteScalarWithConvert |    75.95 ns |  1.383 ns |  1.983 ns |    75.84 ns |    73.74 ns |    78.79 ns |    78.10 ns | 0.0100 |      - |     - |     168 B |
|                 DapperQuery100 | 3,098.22 ns |  7.611 ns | 10.669 ns | 3,098.87 ns | 3,076.88 ns | 3,121.09 ns | 3,109.24 ns | 0.3516 | 0.0039 |     - |    5896 B |
|                  SmartQuery100 | 1,715.03 ns | 18.127 ns | 27.131 ns | 1,711.31 ns | 1,684.95 ns | 1,774.58 ns | 1,752.33 ns | 0.2090 |      - |     - |    3520 B |
|          DapperQuery100Bufferd | 2,622.66 ns | 17.871 ns | 26.195 ns | 2,621.53 ns | 2,579.45 ns | 2,659.61 ns | 2,650.48 ns | 0.3477 | 0.0039 |     - |    5856 B |
|           SmartQuery100Bufferd | 1,771.01 ns | 34.526 ns | 49.517 ns | 1,794.77 ns | 1,716.36 ns | 1,867.76 ns | 1,838.48 ns | 0.3301 | 0.0039 |     - |    5536 B |
|               DapperQueryFirst |   262.39 ns |  0.438 ns |  0.614 ns |   262.38 ns |   261.47 ns |   263.40 ns |   263.23 ns | 0.0200 |      - |     - |     336 B |
|                SmartQueryFirst |   284.45 ns |  0.832 ns |  1.219 ns |   284.39 ns |   281.76 ns |   287.71 ns |   285.72 ns | 0.0146 |      - |     - |     248 B |

Not so late( ˙ω˙)?
