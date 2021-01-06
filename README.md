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

* First result mapper by code generation.

## Benchmark(for reference purpose only)

Benchmark result on .NET Core 5 with Code generation mode.

``` ini

BenchmarkDotNet=v0.12.1, OS=Windows 10.0.19042
AMD Ryzen 9 5900X, 1 CPU, 24 logical and 12 physical cores
.NET Core SDK=5.0.101
  [Host]  : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT
  LongRun : .NET Core 5.0.1 (CoreCLR 5.0.120.57516, CoreFX 5.0.120.57516), X64 RyuJIT

Job=LongRun  IterationCount=100  LaunchCount=3  
WarmupCount=15  

```
|                         Method |        Mean |    Error |    StdDev |      Median |         Min | Allocated |
|------------------------------- |------------:|---------:|----------:|------------:|------------:|----------:|
|                  DapperExecute |   186.80 ns | 2.113 ns | 10.898 ns |   180.34 ns |   175.71 ns |     456 B |
|                   SmartExecute |   108.90 ns | 0.284 ns |  1.447 ns |   108.27 ns |   106.65 ns |     368 B |
|   DapperExecuteWithParameter10 |   465.44 ns | 2.295 ns | 11.777 ns |   463.73 ns |   447.15 ns |    1256 B |
|    SmartExecuteWithParameter10 |   302.43 ns | 0.350 ns |  1.810 ns |   302.26 ns |   298.72 ns |    1256 B |
| DapperExecuteWithOverParameter |   171.27 ns | 1.103 ns |  5.659 ns |   168.14 ns |   165.08 ns |     472 B |
|  SmartExecuteWithOverParameter |   305.39 ns | 1.015 ns |  5.180 ns |   303.70 ns |   297.42 ns |    1256 B |
|            DapperExecuteScalar |    66.65 ns | 0.125 ns |  0.647 ns |    66.63 ns |    65.23 ns |     144 B |
|             SmartExecuteScalar |    53.65 ns | 0.056 ns |  0.287 ns |    53.61 ns |    53.12 ns |     144 B |
| DapperExecuteScalarWithConvert |   117.84 ns | 0.232 ns |  1.186 ns |   118.28 ns |   115.27 ns |     168 B |
|  SmartExecuteScalarWithConvert |    75.14 ns | 0.265 ns |  1.368 ns |    75.50 ns |    72.73 ns |     168 B |
|                 DapperQuery100 | 3,316.67 ns | 4.272 ns | 22.072 ns | 3,320.90 ns | 3,265.06 ns |    5896 B |
|                  SmartQuery100 | 1,909.76 ns | 2.915 ns | 14.641 ns | 1,904.54 ns | 1,887.01 ns |    3520 B |
|          DapperQuery100Bufferd | 2,794.83 ns | 3.068 ns | 15.852 ns | 2,795.25 ns | 2,750.51 ns |    5856 B |
|           SmartQuery100Bufferd | 1,853.40 ns | 8.447 ns | 43.344 ns | 1,832.09 ns | 1,792.46 ns |    5536 B |
|               DapperQueryFirst |   277.25 ns | 0.818 ns |  4.197 ns |   276.58 ns |   270.74 ns |     336 B |
|                SmartQueryFirst |   296.97 ns | 0.505 ns |  2.612 ns |   296.88 ns |   291.68 ns |     248 B |

Not so late( ˙ω˙)?
