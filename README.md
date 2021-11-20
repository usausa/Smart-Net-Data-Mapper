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

Benchmark result on .NET Core 6 with Code generation mode.

``` ini
BenchmarkDotNet=v0.13.1, OS=Windows 10.0.22000
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
.NET SDK=6.0.100
  [Host]    : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT
  MediumRun : .NET 6.0.0 (6.0.21.52210), X64 RyuJIT

Job=MediumRun  IterationCount=15  LaunchCount=2  
WarmupCount=10  
```
|                         Method |        Mean |     Error |    StdDev |      Median |         Min |         Max |         P90 |  Gen 0 |  Gen 1 | Allocated |
|------------------------------- |------------:|----------:|----------:|------------:|------------:|------------:|------------:|-------:|-------:|----------:|
|                  DapperExecute |   199.38 ns |  4.410 ns |  6.325 ns |   198.85 ns |   191.86 ns |   207.80 ns |   206.09 ns | 0.0271 |      - |     456 B |
|                   SmartExecute |   112.21 ns |  0.487 ns |  0.714 ns |   112.30 ns |   111.13 ns |   113.36 ns |   113.20 ns | 0.0220 |      - |     368 B |
|   DapperExecuteWithParameter10 |   465.05 ns |  2.959 ns |  4.429 ns |   465.76 ns |   456.20 ns |   473.15 ns |   471.33 ns | 0.0747 |      - |   1,256 B |
|    SmartExecuteWithParameter10 |   297.31 ns |  1.206 ns |  1.805 ns |   297.67 ns |   294.21 ns |   300.14 ns |   299.33 ns | 0.0747 |      - |   1,256 B |
| DapperExecuteWithOverParameter |   211.95 ns |  5.931 ns |  8.878 ns |   211.56 ns |   201.79 ns |   222.65 ns |   221.30 ns | 0.0281 |      - |     472 B |
|  SmartExecuteWithOverParameter |   303.41 ns |  3.547 ns |  4.855 ns |   302.71 ns |   297.49 ns |   311.91 ns |   309.95 ns | 0.0747 |      - |   1,256 B |
|            DapperExecuteScalar |    71.76 ns |  0.471 ns |  0.675 ns |    71.73 ns |    70.75 ns |    73.29 ns |    72.61 ns | 0.0085 |      - |     144 B |
|             SmartExecuteScalar |    57.86 ns |  0.125 ns |  0.187 ns |    57.90 ns |    57.53 ns |    58.21 ns |    58.08 ns | 0.0086 |      - |     144 B |
| DapperExecuteScalarWithConvert |   115.50 ns |  0.818 ns |  1.224 ns |   115.53 ns |   112.94 ns |   117.78 ns |   117.03 ns | 0.0100 |      - |     168 B |
|  SmartExecuteScalarWithConvert |    95.39 ns |  8.754 ns | 12.555 ns |   103.46 ns |    81.43 ns |   109.65 ns |   108.71 ns | 0.0100 |      - |     168 B |
|                 DapperQuery100 | 3,050.47 ns | 24.408 ns | 36.533 ns | 3,049.91 ns | 2,987.18 ns | 3,111.94 ns | 3,095.05 ns | 0.3516 | 0.0039 |   5,896 B |
|                  SmartQuery100 | 1,740.79 ns | 15.799 ns | 23.158 ns | 1,739.19 ns | 1,696.38 ns | 1,776.15 ns | 1,766.91 ns | 0.2090 |      - |   3,520 B |
|          DapperQuery100Bufferd | 2,624.62 ns | 14.147 ns | 21.174 ns | 2,631.04 ns | 2,567.28 ns | 2,654.26 ns | 2,643.83 ns | 0.3477 | 0.0039 |   5,856 B |
|           SmartQuery100Bufferd | 1,791.79 ns | 11.596 ns | 17.356 ns | 1,796.36 ns | 1,765.80 ns | 1,820.38 ns | 1,811.66 ns | 0.3301 | 0.0039 |   5,536 B |
|               DapperQueryFirst |   283.08 ns |  7.661 ns | 11.467 ns |   283.95 ns |   268.49 ns |   295.65 ns |   295.36 ns | 0.0200 |      - |     336 B |
|                SmartQueryFirst |   295.49 ns |  1.218 ns |  1.822 ns |   295.74 ns |   292.29 ns |   298.27 ns |   297.62 ns | 0.0146 |      - |     248 B |

Not so late( ˙ω˙)?
