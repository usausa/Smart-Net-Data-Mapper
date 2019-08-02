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

|                         Method |       Mean |     Error |    StdDev |     Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------- |-----------:|----------:|----------:|-----------:|-------:|------:|------:|----------:|
|                  DapperExecute |   352.9 ns | 0.8447 ns |  4.311 ns |   351.8 ns | 0.1101 |     - |     - |     464 B |
|                   SmartExecute |   194.7 ns | 0.4167 ns |  2.046 ns |   194.9 ns | 0.0894 |     - |     - |     376 B |
|   DapperExecuteWithParameter10 |   767.1 ns | 0.9446 ns |  4.889 ns |   766.5 ns | 0.3004 |     - |     - |    1264 B |
|    SmartExecuteWithParameter10 |   516.0 ns | 0.6310 ns |  3.238 ns |   515.8 ns | 0.3004 |     - |     - |    1264 B |
| DapperExecuteWithOverParameter |   321.8 ns | 0.6742 ns |  3.349 ns |   321.6 ns | 0.1140 |     - |     - |     480 B |
|  SmartExecuteWithOverParameter |   514.7 ns | 0.5797 ns |  2.990 ns |   514.4 ns | 0.3004 |     - |     - |    1264 B |
|            DapperExecuteScalar |   138.3 ns | 0.2828 ns |  1.444 ns |   138.5 ns | 0.0362 |     - |     - |     152 B |
|             SmartExecuteScalar |   110.5 ns | 0.2775 ns |  1.419 ns |   110.1 ns | 0.0362 |     - |     - |     152 B |
| DapperExecuteScalarWithConvert |   234.3 ns | 0.2713 ns |  1.400 ns |   234.1 ns | 0.0417 |     - |     - |     176 B |
|  SmartExecuteScalarWithConvert |   157.4 ns | 0.2090 ns |  1.071 ns |   157.4 ns | 0.0417 |     - |     - |     176 B |
|                 DapperQuery100 | 5,246.0 ns | 6.0221 ns | 30.848 ns | 5,238.5 ns | 1.4038 |     - |     - |    5920 B |
|                  SmartQuery100 | 3,153.3 ns | 3.0727 ns | 15.822 ns | 3,151.9 ns | 0.8202 |     - |     - |    3448 B |
|          DapperQuery100Bufferd | 4,243.6 ns | 5.0693 ns | 26.058 ns | 4,239.2 ns | 1.3962 |     - |     - |    5880 B |
|           SmartQuery100Bufferd | 2,961.4 ns | 3.3767 ns | 17.327 ns | 2,957.9 ns | 1.3199 |     - |     - |    5552 B |
|               DapperQueryFirst |   481.0 ns | 0.5978 ns |  3.089 ns |   480.9 ns | 0.0811 |     - |     - |     344 B |
|                SmartQueryFirst |   630.9 ns | 0.8443 ns |  4.279 ns |   630.3 ns | 0.0601 |     - |     - |     256 B |

Not so late( ˙ω˙)?
