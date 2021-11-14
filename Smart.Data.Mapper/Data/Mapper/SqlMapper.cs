namespace Smart.Data.Mapper
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using System.Threading.Tasks;

    using Smart.Data.Mapper.Parameters;

    public static class SqlMapper
    {
        private const CommandBehavior CommandBehaviorQuery =
            CommandBehavior.SequentialAccess;

        private const CommandBehavior CommandBehaviorQueryFirstOrDefault =
            CommandBehavior.SequentialAccess | CommandBehavior.SingleRow;

        private static readonly ParameterBuilder NullParameterBuilder = new(null, null);

        //--------------------------------------------------------------------------------
        // Core
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities", Justification = "Extension")]
        private static DbCommand SetupCommand(DbConnection con, DbTransaction? transaction, string sql, int? commandTimeout, CommandType? commandType)
        {
            var cmd = con.CreateCommand();

            if (transaction is not null)
            {
                cmd.Transaction = transaction;
            }

            cmd.CommandText = sql;

            if (commandTimeout.HasValue)
            {
                cmd.CommandTimeout = commandTimeout.Value;
            }

            if (commandType.HasValue)
            {
                cmd.CommandType = commandType.Value;
            }

            return cmd;
        }

        //--------------------------------------------------------------------------------
        // Execute
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static int Execute(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param!);

                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    var result = cmd.ExecuteNonQuery();

                    builder.PostProcess?.Invoke(cmd, param!);

                    return result;
                }
                finally
                {
                    if (wasClosed)
                    {
                        con.Close();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Execute(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Execute(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async ValueTask<int> ExecuteAsync(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            await using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param!);

                try
                {
                    if (wasClosed)
                    {
                        await con.OpenAsync(cancel).ConfigureAwait(false);
                    }

                    var result = await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);

                    builder.PostProcess?.Invoke(cmd, param!);

                    return result;
                }
                finally
                {
                    if (wasClosed)
                    {
                        await con.CloseAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<int> ExecuteAsync(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            return ExecuteAsync(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, cancel);
        }

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static T ExecuteScalar<T>(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param!);

                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    var result = cmd.ExecuteScalar();

                    builder.PostProcess?.Invoke(cmd, param!);

                    if (result is T scalar)
                    {
                        return scalar;
                    }

                    if (result is null)
                    {
                        return default!;
                    }

                    return config.Convert<T>(result);
                }
                finally
                {
                    if (wasClosed)
                    {
                        con.Close();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T ExecuteScalar<T>(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return ExecuteScalar<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async ValueTask<T> ExecuteScalarAsync<T>(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            await using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param!);

                try
                {
                    if (wasClosed)
                    {
                        await con.OpenAsync(cancel).ConfigureAwait(false);
                    }

                    var result = await cmd.ExecuteScalarAsync(cancel).ConfigureAwait(false);

                    builder.PostProcess?.Invoke(cmd, param!);

                    if (result is T scalar)
                    {
                        return scalar;
                    }

                    if (result is null)
                    {
                        return default!;
                    }

                    return config.Convert<T>(result);
                }
                finally
                {
                    if (wasClosed)
                    {
                        await con.CloseAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<T> ExecuteScalarAsync<T>(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            return ExecuteScalarAsync<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, cancel);
        }

        //--------------------------------------------------------------------------------
        // ExecuteReader
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static IDataReader ExecuteReader(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var cmd = default(DbCommand);
            var reader = default(DbDataReader);
            try
            {
                cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType);
                var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param!);

                if (wasClosed)
                {
                    con.Open();
                }

                reader = cmd.ExecuteReader(wasClosed ? commandBehavior | CommandBehavior.CloseConnection : commandBehavior);
                wasClosed = false;

                builder.PostProcess?.Invoke(cmd, param!);

                return new WrappedReader(cmd, reader);
            }
            catch (Exception)
            {
                reader?.Dispose();
                cmd?.Dispose();
                throw;
            }
            finally
            {
                if (wasClosed)
                {
                    con.Close();
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IDataReader ExecuteReader(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            return ExecuteReader(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, commandBehavior);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async ValueTask<IDataReader> ExecuteReaderAsync(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var cmd = default(DbCommand);
            var reader = default(DbDataReader);
            try
            {
                cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType);
                var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param!);

                if (wasClosed)
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);
                }

                reader = await cmd.ExecuteReaderAsync(wasClosed ? commandBehavior | CommandBehavior.CloseConnection : commandBehavior, cancel).ConfigureAwait(false);
                wasClosed = false;

                builder.PostProcess?.Invoke(cmd, param!);

                return new WrappedReader(cmd, reader);
            }
            catch (Exception)
            {
                if (reader is not null)
                {
                    await reader.DisposeAsync().ConfigureAwait(false);
                }
                cmd?.Dispose();
                throw;
            }
            finally
            {
                if (wasClosed)
                {
                    await con.CloseAsync().ConfigureAwait(false);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<IDataReader> ExecuteReaderAsync(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken cancel = default)
        {
            return ExecuteReaderAsync(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, commandBehavior, cancel);
        }

        //--------------------------------------------------------------------------------
        // Query
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static IEnumerable<T> Query<T>(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param!);

                if (wasClosed)
                {
                    con.Open();
                }

                try
                {
                    using (var reader = cmd.ExecuteReader(CommandBehaviorQuery))
                    {
                        builder.PostProcess?.Invoke(cmd, param!);

                        if (reader.Read())
                        {
                            var mapper = config.CreateResultMapper<T>(reader);

                            do
                            {
                                yield return mapper(reader);
                            }
                            while (reader.Read());
                        }
                    }
                }
                finally
                {
                    if (wasClosed)
                    {
                        con.Close();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Query<T>(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Query<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async IAsyncEnumerable<T> QueryAsync<T>(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, [EnumeratorCancellation] CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            try
            {
                await using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
                {
                    var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                    builder.Build?.Invoke(cmd, param!);

                    if (wasClosed)
                    {
                        await con.OpenAsync(cancel).ConfigureAwait(false);
                    }

                    await using (var reader = await cmd.ExecuteReaderAsync(CommandBehaviorQuery, cancel).ConfigureAwait(false))
                    {
                        builder.PostProcess?.Invoke(cmd, param!);

                        if (await reader.ReadAsync(cancel).ConfigureAwait(false))
                        {
                            var mapper = config.CreateResultMapper<T>(reader);

                            do
                            {
                                yield return mapper(reader);
                            }
                            while (await reader.ReadAsync(cancel).ConfigureAwait(false));
                        }
                    }
                }
            }
            finally
            {
                if (wasClosed)
                {
                    await con.CloseAsync().ConfigureAwait(false);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IAsyncEnumerable<T> QueryAsync<T>(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            return QueryAsync<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, cancel);
        }

        //--------------------------------------------------------------------------------
        // QueryList
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:DoNotExposeGenericLists", Justification = "Performance")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static List<T> QueryList<T>(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param!);

                if (wasClosed)
                {
                    con.Open();
                }

                try
                {
                    using (var reader = cmd.ExecuteReader(CommandBehaviorQuery))
                    {
                        builder.PostProcess?.Invoke(cmd, param!);

                        var list = new List<T>();
                        if (reader.Read())
                        {
                            var mapper = config.CreateResultMapper<T>(reader);

                            do
                            {
                                list.Add(mapper(reader));
                            }
                            while (reader.Read());
                        }

                        return list;
                    }
                }
                finally
                {
                    if (wasClosed)
                    {
                        con.Close();
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1002:DoNotExposeGenericLists", Justification = "Performance")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> QueryList<T>(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return QueryList<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async ValueTask<List<T>> QueryListAsync<T>(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            await using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param!);

                if (wasClosed)
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);
                }

                try
                {
                    await using (var reader = await cmd.ExecuteReaderAsync(CommandBehaviorQuery, cancel).ConfigureAwait(false))
                    {
                        builder.PostProcess?.Invoke(cmd, param!);

                        var list = new List<T>();
                        if (await reader.ReadAsync(cancel).ConfigureAwait(false))
                        {
                            var mapper = config.CreateResultMapper<T>(reader);

                            do
                            {
                                list.Add(mapper(reader));
                            }
                            while (await reader.ReadAsync(cancel).ConfigureAwait(false));
                        }

                        return list;
                    }
                }
                finally
                {
                    if (wasClosed)
                    {
                        await con.CloseAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<List<T>> QueryListAsync<T>(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            return QueryListAsync<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, cancel);
        }

        //--------------------------------------------------------------------------------
        // QueryFirstOrDefault
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static T? QueryFirstOrDefault<T>(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param!);

                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    using (var reader = cmd.ExecuteReader(CommandBehaviorQueryFirstOrDefault))
                    {
                        builder.PostProcess?.Invoke(cmd, param!);

                        if (reader.Read())
                        {
                            var mapper = config.CreateResultMapper<T>(reader);
                            return mapper(reader);
                        }

                        return default;
                    }
                }
                finally
                {
                    if (wasClosed)
                    {
                        con.Close();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T? QueryFirstOrDefault<T>(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return QueryFirstOrDefault<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async ValueTask<T?> QueryFirstOrDefaultAsync<T>(this DbConnection con, ISqlMapperConfig config, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            await using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param is not null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param!);

                try
                {
                    if (wasClosed)
                    {
                        await con.OpenAsync(cancel).ConfigureAwait(false);
                    }

                    await using (var reader = await cmd.ExecuteReaderAsync(CommandBehaviorQueryFirstOrDefault, cancel).ConfigureAwait(false))
                    {
                        builder.PostProcess?.Invoke(cmd, param!);

                        if (await reader.ReadAsync(cancel).ConfigureAwait(false))
                        {
                            var mapper = config.CreateResultMapper<T>(reader);
                            return mapper(reader);
                        }

                        return default;
                    }
                }
                finally
                {
                    if (wasClosed)
                    {
                        await con.CloseAsync().ConfigureAwait(false);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ValueTask<T?> QueryFirstOrDefaultAsync<T>(this DbConnection con, string sql, object? param = null, DbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            return QueryFirstOrDefaultAsync<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, cancel);
        }
    }
}
