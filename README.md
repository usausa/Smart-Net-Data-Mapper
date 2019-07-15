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

|                         Method |       Mean |     Error |     StdDev |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------- |-----------:|----------:|-----------:|-------:|------:|------:|----------:|
|                  DapperExecute |   391.0 ns |  4.081 ns |   5.853 ns | 0.1101 |     - |     - |     464 B |
|                   SmartExecute |   217.9 ns |  1.537 ns |   2.301 ns | 0.0894 |     - |     - |     376 B |
|   DapperExecuteWithParameter10 |   864.9 ns |  5.297 ns |   7.765 ns | 0.3004 |     - |     - |    1264 B |
|    SmartExecuteWithParameter10 |   594.8 ns |  5.573 ns |   8.169 ns | 0.3004 |     - |     - |    1264 B |
| DapperExecuteWithOverParameter |   365.4 ns |  3.468 ns |   4.862 ns | 0.1140 |     - |     - |     480 B |
|  SmartExecuteWithOverParameter |   592.8 ns |  3.984 ns |   5.964 ns | 0.3004 |     - |     - |    1264 B |
|            DapperExecuteScalar |   153.8 ns |  1.742 ns |   2.607 ns | 0.0362 |     - |     - |     152 B |
|             SmartExecuteScalar |   124.2 ns |  1.310 ns |   1.920 ns | 0.0362 |     - |     - |     152 B |
| DapperExecuteScalarWithConvert |   262.5 ns |  7.913 ns |  10.831 ns | 0.0415 |     - |     - |     176 B |
|  SmartExecuteScalarWithConvert |   239.6 ns |  2.029 ns |   3.038 ns | 0.0741 |     - |     - |     312 B |
|                 DapperQuery100 | 5,923.4 ns | 68.810 ns | 100.860 ns | 1.4038 |     - |     - |    5920 B |
|                  SmartQuery100 | 3,651.7 ns | 35.622 ns |  49.937 ns | 0.8202 |     - |     - |    3448 B |
|          DapperQuery100Bufferd | 4,723.7 ns | 28.598 ns |  41.015 ns | 1.3962 |     - |     - |    5880 B |
|           SmartQuery100Bufferd | 3,311.4 ns | 13.551 ns |  19.862 ns | 1.3199 |     - |     - |    5552 B |
|               DapperQueryFirst |   531.5 ns |  4.330 ns |   6.347 ns | 0.0811 |     - |     - |     344 B |
|                SmartQueryFirst |   693.2 ns |  2.772 ns |   4.149 ns | 0.0601 |     - |     - |     256 B |

Not so late( ˙ω˙)?
