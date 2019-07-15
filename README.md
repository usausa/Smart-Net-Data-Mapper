# Smart.Data.Mapper .NET - micro-orm library for .NET

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

| Id                    |
|-----------------------|
| Usa.Smart.Data.Mapper |

## Functions

### Execute

```csharp
int Execute(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

int Execute(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

async Task<int> ExecuteAsync(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = default)

Task<int> ExecuteAsync(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = default)
```

### ExecuteScalar

```csharp
T ExecuteScalar<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

T ExecuteScalar<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

async Task<T> ExecuteScalarAsync<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = default)

Task<T> ExecuteScalarAsync<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = default)
```

### ExecuteReader

```csharp
IDataReader ExecuteReader(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default)

IDataReader ExecuteReader(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default)

async Task<DbDataReader> ExecuteReaderAsync(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken token = default)

Task<DbDataReader> ExecuteReaderAsync(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken token = default)
```

### Query

```csharp
IEnumerable<T> Query<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

IEnumerable<T> Query<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

async Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)

Task<IEnumerable<T>> QueryAsync<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
```

### QueryList

```csharp
List<T> QueryList<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

List<T> QueryList<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

async Task<List<T>> QueryListAsync<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)

Task<List<T>> QueryListAsync<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
```

### QueryFirstOrDefault

```csharp
T QueryFirstOrDefault<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

T QueryFirstOrDefault<T>(this IDbConnection con, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)

async Task<T> QueryFirstOrDefaultAsync<T>(this IDbConnection con, ISqlMapperConfig config, string sql, object param = null, IDbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken token = default)

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

Benchmark result on .NET Core 2.2 with Code generation mode.

|                         Method |       Mean |      Error |    StdDev |     Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------- |-----------:|-----------:|----------:|-----------:|-------:|------:|------:|----------:|
|                  DapperExecute |   397.1 ns |  1.1817 ns |  5.946 ns |   396.0 ns | 0.1101 |     - |     - |     464 B |
|                   SmartExecute |   221.9 ns |  0.6833 ns |  3.469 ns |   221.5 ns | 0.0894 |     - |     - |     376 B |
|   DapperExecuteWithParameter10 |   885.8 ns |  1.6714 ns |  8.547 ns |   885.6 ns | 0.3004 |     - |     - |    1264 B |
|    SmartExecuteWithParameter10 |   598.6 ns |  1.4634 ns |  7.350 ns |   596.8 ns | 0.3004 |     - |     - |    1264 B |
| DapperExecuteWithOverParameter |   363.8 ns |  1.0058 ns |  5.097 ns |   363.2 ns | 0.1140 |     - |     - |     480 B |
|  SmartExecuteWithOverParameter |   604.7 ns |  1.7266 ns |  8.687 ns |   602.9 ns | 0.3004 |     - |     - |    1264 B |
|            DapperExecuteScalar |   154.6 ns |  0.4369 ns |  2.182 ns |   154.6 ns | 0.0362 |     - |     - |     152 B |
|             SmartExecuteScalar |   125.1 ns |  0.5660 ns |  2.801 ns |   124.5 ns | 0.0362 |     - |     - |     152 B |
| DapperExecuteScalarWithConvert |   261.8 ns |  0.6669 ns |  3.349 ns |   261.6 ns | 0.0415 |     - |     - |     176 B |
|  SmartExecuteScalarWithConvert |   239.5 ns |  0.6712 ns |  3.402 ns |   238.7 ns | 0.0739 |     - |     - |     312 B |
|                 DapperQuery100 | 5,846.9 ns | 13.9192 ns | 69.777 ns | 5,840.2 ns | 1.4038 |     - |     - |    5920 B |
|                  SmartQuery100 | 4,555.5 ns | 12.1105 ns | 61.155 ns | 4,541.8 ns | 0.8163 |     - |     - |    3448 B |
|          DapperQuery100Bufferd | 4,881.8 ns | 12.5583 ns | 63.760 ns | 4,864.0 ns | 1.3962 |     - |     - |    5880 B |
|           SmartQuery100Bufferd | 4,342.0 ns |  9.3063 ns | 46.994 ns | 4,334.1 ns | 1.3199 |     - |     - |    5552 B |
|               DapperQueryFirst |   534.9 ns |  1.1708 ns |  5.912 ns |   533.9 ns | 0.0811 |     - |     - |     344 B |
|                SmartQueryFirst |   737.6 ns |  1.5840 ns |  8.085 ns |   735.9 ns | 0.0601 |     - |     - |     256 B |

Not so late( ˙ω˙)?
