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

## Benchmark(for reference purpose only)

Benchmark result on .NET Core 2.2 with Code generation mode.

|                         Method |       Mean |      Error |    StdDev |     Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|------------------------------- |-----------:|-----------:|----------:|-----------:|-------:|------:|------:|----------:|
|                  DapperExecute |   388.5 ns |  0.6510 ns |  3.305 ns |   388.2 ns | 0.1101 |     - |     - |     464 B |
|                   SmartExecute |   243.6 ns |  0.4917 ns |  2.483 ns |   243.2 ns | 0.0892 |     - |     - |     376 B |
|   DapperExecuteWithParameter10 |   878.0 ns |  1.2811 ns |  6.585 ns |   878.0 ns | 0.3004 |     - |     - |    1264 B |
|    SmartExecuteWithParameter10 |   704.0 ns |  0.9440 ns |  4.818 ns |   703.2 ns | 0.3004 |     - |     - |    1264 B |
| DapperExecuteWithOverParameter |   357.4 ns |  0.6111 ns |  3.108 ns |   357.2 ns | 0.1140 |     - |     - |     480 B |
|  SmartExecuteWithOverParameter |   707.5 ns |  1.2544 ns |  6.392 ns |   706.1 ns | 0.3004 |     - |     - |    1264 B |
|            DapperExecuteScalar |   151.5 ns |  0.3255 ns |  1.664 ns |   151.3 ns | 0.0362 |     - |     - |     152 B |
|             SmartExecuteScalar |   128.6 ns |  0.3784 ns |  1.911 ns |   128.0 ns | 0.0362 |     - |     - |     152 B |
| DapperExecuteScalarWithConvert |   255.5 ns |  0.3646 ns |  1.864 ns |   255.3 ns | 0.0415 |     - |     - |     176 B |
|  SmartExecuteScalarWithConvert |   240.4 ns |  0.4056 ns |  2.081 ns |   240.1 ns | 0.0741 |     - |     - |     312 B |
|                 DapperQuery100 | 5,751.1 ns | 10.6433 ns | 54.230 ns | 5,745.5 ns | 1.4038 |     - |     - |    5920 B |
|                  SmartQuery100 | 4,395.3 ns |  7.8120 ns | 39.377 ns | 4,386.7 ns | 0.8163 |     - |     - |    3448 B |
|          DapperQuery100Bufferd | 4,797.6 ns |  6.7593 ns | 33.822 ns | 4,794.4 ns | 1.3962 |     - |     - |    5880 B |
|           SmartQuery100Bufferd | 4,459.3 ns |  5.9561 ns | 30.348 ns | 4,455.1 ns | 1.3199 |     - |     - |    5552 B |
|               DapperQueryFirst |   525.2 ns |  0.7646 ns |  3.882 ns |   524.7 ns | 0.0811 |     - |     - |     344 B |
|                SmartQueryFirst |   727.1 ns |  0.7818 ns |  3.948 ns |   726.6 ns | 0.0601 |     - |     - |     256 B |

Not so late( ˙ω˙)?
