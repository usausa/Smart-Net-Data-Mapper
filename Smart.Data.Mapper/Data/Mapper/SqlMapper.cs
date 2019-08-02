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
        private const CommandBehavior CommandBehaviorQueryWithClose =
            CommandBehavior.SequentialAccess | CommandBehavior.CloseConnection;

        private const CommandBehavior CommandBehaviorQuery =
            CommandBehavior.SequentialAccess;

        private const CommandBehavior CommandBehaviorQueryFirstOrDefault =
            CommandBehavior.SequentialAccess | CommandBehavior.SingleRow;

        private static readonly ParameterBuilder NullParameterBuilder = new ParameterBuilder(null, null);

        //--------------------------------------------------------------------------------
        // Core
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:ReviewSqlQueriesForSecurityVulnerabilities", Justification = "Extension")]
        private static DbCommand SetupCommand(DbConnection con, DbTransaction transaction, string sql, int? commandTimeout, CommandType? commandType)
        {
            var cmd = con.CreateCommand();

            if (transaction != null)
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
        public static int Execute(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    var result = cmd.ExecuteNonQuery();

                    builder.PostProcess?.Invoke(cmd, param);

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
        public static int Execute(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Execute(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async Task<int> ExecuteAsync(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                try
                {
                    if (wasClosed)
                    {
                        await con.OpenAsync(cancel).ConfigureAwait(false);
                    }

                    var result = await cmd.ExecuteNonQueryAsync(cancel).ConfigureAwait(false);

                    builder.PostProcess?.Invoke(cmd, param);

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
        public static Task<int> ExecuteAsync(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            return ExecuteAsync(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, cancel);
        }

        //--------------------------------------------------------------------------------
        // ExecuteScalar
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static T ExecuteScalar<T>(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    var result = cmd.ExecuteScalar();

                    builder.PostProcess?.Invoke(cmd, param);

                    if (result is T scalar)
                    {
                        return scalar;
                    }

                    if (result is DBNull)
                    {
                        return default;
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
        public static T ExecuteScalar<T>(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return ExecuteScalar<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async Task<T> ExecuteScalarAsync<T>(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                try
                {
                    if (wasClosed)
                    {
                        await con.OpenAsync(cancel).ConfigureAwait(false);
                    }

                    var result = await cmd.ExecuteScalarAsync(cancel).ConfigureAwait(false);

                    builder.PostProcess?.Invoke(cmd, param);

                    if (result is T scalar)
                    {
                        return scalar;
                    }

                    if (result is DBNull)
                    {
                        return default;
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
        public static Task<T> ExecuteScalarAsync<T>(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            return ExecuteScalarAsync<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, cancel);
        }

        //--------------------------------------------------------------------------------
        // ExecuteReader
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static IDataReader ExecuteReader(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var cmd = default(DbCommand);
            var reader = default(DbDataReader);
            try
            {
                cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType);
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                if (wasClosed)
                {
                    con.Open();
                }

                reader = cmd.ExecuteReader(wasClosed
                    ? commandBehavior | CommandBehavior.CloseConnection
                    : commandBehavior);
                wasClosed = false;

                builder.PostProcess?.Invoke(cmd, param);

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
        public static IDataReader ExecuteReader(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default)
        {
            return ExecuteReader(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, commandBehavior);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async Task<IDataReader> ExecuteReaderAsync(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var cmd = default(DbCommand);
            var reader = default(DbDataReader);
            try
            {
                cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType);
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                if (wasClosed)
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);
                }

                reader = await cmd.ExecuteReaderAsync(wasClosed ? commandBehavior | CommandBehavior.CloseConnection : commandBehavior, cancel).ConfigureAwait(false);
                wasClosed = false;

                builder.PostProcess?.Invoke(cmd, param);

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
        public static Task<IDataReader> ExecuteReaderAsync(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CommandBehavior commandBehavior = CommandBehavior.Default, CancellationToken cancel = default)
        {
            return ExecuteReaderAsync(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, commandBehavior, cancel);
        }

        //--------------------------------------------------------------------------------
        // Query
        //--------------------------------------------------------------------------------

        private static IEnumerable<T> ReaderToDefer<T>(DbCommand cmd, IDataReader reader, Func<IDataRecord, T> mapper)
        {
            using (cmd)
            using (reader)
            {
                while (reader.Read())
                {
                    yield return mapper(reader);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static IEnumerable<T> Query<T>(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var cmd = default(DbCommand);
            var reader = default(IDataReader);
            try
            {
                cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType);
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                if (wasClosed)
                {
                    con.Open();
                }

                reader = cmd.ExecuteReader(wasClosed ? CommandBehaviorQueryWithClose : CommandBehaviorQuery);
                wasClosed = false;

                builder.PostProcess?.Invoke(cmd, param);

                var mapper = config.CreateResultMapper<T>(reader);

                var deferred = ReaderToDefer(cmd, reader, mapper);
                cmd = null;
                reader = null;
                return deferred;
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
        public static IEnumerable<T> Query<T>(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return Query<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async Task<IEnumerable<T>> QueryAsync<T>(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            var cmd = default(DbCommand);
            var reader = default(DbDataReader);
            try
            {
                cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType);
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                if (wasClosed)
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);
                }

                reader = await cmd.ExecuteReaderAsync(wasClosed ? CommandBehaviorQueryWithClose : CommandBehaviorQuery, cancel).ConfigureAwait(false);
                wasClosed = false;

                builder.PostProcess?.Invoke(cmd, param);

                var mapper = config.CreateResultMapper<T>(reader);

                var deferred = ReaderToDefer(cmd, reader, mapper);
                cmd = null;
                reader = null;
                return deferred;
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
        public static Task<IEnumerable<T>> QueryAsync<T>(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            return QueryAsync<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, cancel);
        }

        //--------------------------------------------------------------------------------
        // QueryList
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static List<T> QueryList<T>(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                if (wasClosed)
                {
                    con.Open();
                }

                try
                {
                    using (var reader = cmd.ExecuteReader(CommandBehaviorQuery))
                    {
                        builder.PostProcess?.Invoke(cmd, param);

                        var mapper = config.CreateResultMapper<T>(reader);

                        var list = new List<T>();
                        while (reader.Read())
                        {
                            list.Add(mapper(reader));
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static List<T> QueryList<T>(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return QueryList<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async Task<List<T>> QueryListAsync<T>(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                if (wasClosed)
                {
                    await con.OpenAsync(cancel).ConfigureAwait(false);
                }

                try
                {
                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehaviorQuery, cancel).ConfigureAwait(false))
                    {
                        builder.PostProcess?.Invoke(cmd, param);

                        var mapper = config.CreateResultMapper<T>(reader);

                        var list = new List<T>();
                        while (await reader.ReadAsync(cancel).ConfigureAwait(false))
                        {
                            list.Add(mapper(reader));
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<List<T>> QueryListAsync<T>(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            return QueryListAsync<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, cancel);
        }

        //--------------------------------------------------------------------------------
        // QueryFirstOrDefault
        //--------------------------------------------------------------------------------

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static T QueryFirstOrDefault<T>(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                try
                {
                    if (wasClosed)
                    {
                        con.Open();
                    }

                    using (var reader = cmd.ExecuteReader(CommandBehaviorQueryFirstOrDefault))
                    {
                        builder.PostProcess?.Invoke(cmd, param);

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
        public static T QueryFirstOrDefault<T>(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null)
        {
            return QueryFirstOrDefault<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:ValidateArgumentsOfPublicMethods", Justification = "Extension")]
        public static async Task<T> QueryFirstOrDefaultAsync<T>(this DbConnection con, ISqlMapperConfig config, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            var wasClosed = con.State == ConnectionState.Closed;
            using (var cmd = SetupCommand(con, transaction, sql, commandTimeout, commandType))
            {
                var builder = param != null ? config.CreateParameterBuilder(param.GetType()) : NullParameterBuilder;
                builder.Build?.Invoke(cmd, param);

                try
                {
                    if (wasClosed)
                    {
                        await con.OpenAsync(cancel).ConfigureAwait(false);
                    }

                    using (var reader = await cmd.ExecuteReaderAsync(CommandBehaviorQueryFirstOrDefault, cancel).ConfigureAwait(false))
                    {
                        builder.PostProcess?.Invoke(cmd, param);

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
                        con.Close();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task<T> QueryFirstOrDefaultAsync<T>(this DbConnection con, string sql, object param = null, DbTransaction transaction = null, int? commandTimeout = null, CommandType? commandType = null, CancellationToken cancel = default)
        {
            return QueryFirstOrDefaultAsync<T>(con, SqlMapperConfig.Default, sql, param, transaction, commandTimeout, commandType, cancel);
        }
    }
}
