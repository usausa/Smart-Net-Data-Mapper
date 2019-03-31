namespace DataAccess.FormsApp.Modules
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using DataAccess.FormsApp.Components;
    using DataAccess.FormsApp.Models;

    using Smart.ComponentModel;
    using Smart.Data;
    using Smart.Data.Mapper;
    //using Smart.Data.Mapper.Builders;
    using Smart.Forms.Input;
    using Smart.Navigation;

    using Microsoft.Data.Sqlite;

    public class MenuViewModel : AppViewModelBase
    {
        public static MenuViewModel DesignInstance { get; } = null; // For design

        private readonly Settings settings;

        private readonly IDialogs dialogs;

        private readonly IConnectionFactory connectionFactory;

        private readonly SqliteConnection memoryConnection;

        public NotificationValue<bool> IsCreated { get; } = new NotificationValue<bool>();

        public NotificationValue<bool> IsInserted { get; } = new NotificationValue<bool>();

        public NotificationValue<bool> IsMemoryInserted { get; } = new NotificationValue<bool>();

        public AsyncCommand CreateCommand { get; }
        public DelegateCommand DropCommand { get; }
        public AsyncCommand InsertCommand { get; }
        public AsyncCommand UpdateCommand { get; }
        public AsyncCommand DeleteCommand { get; }
        public AsyncCommand CountCommand { get; }
        public AsyncCommand Select1Command { get; }
        public AsyncCommand SelectAllCommand { get; }
        public AsyncCommand InsertBulkCommand { get; }
        public AsyncCommand DeleteAllCommand { get; }
        public AsyncCommand MemoryInsertBulkCommand { get; }
        public AsyncCommand MemoryDeleteAllCommand { get; }

        public MenuViewModel(
            ApplicationState applicationState,
            Settings settings,
            IDialogs dialogs,
            IConnectionFactory connectionFactory)
            : base(applicationState)
        {
            this.settings = settings;
            this.dialogs = dialogs;
            this.connectionFactory = connectionFactory;

            CreateCommand = MakeAsyncCommand(Create, () => !IsCreated.Value).Observe(IsCreated);
            DropCommand = MakeDelegateCommand(Drop, () => IsCreated.Value).Observe(IsCreated);
            InsertCommand = MakeAsyncCommand(Insert, () => IsCreated.Value).Observe(IsCreated);
            UpdateCommand = MakeAsyncCommand(Update, () => IsCreated.Value).Observe(IsCreated);
            DeleteCommand = MakeAsyncCommand(Delete, () => IsCreated.Value).Observe(IsCreated);
            CountCommand = MakeAsyncCommand(Count, () => IsCreated.Value).Observe(IsCreated);
            Select1Command = MakeAsyncCommand(Select1, () => IsCreated.Value).Observe(IsCreated);
            SelectAllCommand = MakeAsyncCommand(SelectAll, () => IsCreated.Value).Observe(IsCreated);
            InsertBulkCommand = MakeAsyncCommand(InsertBulk, () => !IsInserted.Value).Observe(IsInserted);
            DeleteAllCommand = MakeAsyncCommand(DeleteAll, () => IsInserted.Value).Observe(IsCreated);
            MemoryInsertBulkCommand = MakeAsyncCommand(MemoryInsertBulk, () => !IsMemoryInserted.Value).Observe(IsMemoryInserted);
            MemoryDeleteAllCommand = MakeAsyncCommand(MemoryDeleteAll, () => IsMemoryInserted.Value).Observe(IsMemoryInserted);

            IsCreated.Value = File.Exists(settings.DatabasePath);

            memoryConnection = new SqliteConnection("Data Source=:memory:");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                memoryConnection.Dispose();
            }

            base.Dispose(disposing);
        }

        public override async void OnNavigatingTo(INavigationContext context)
        {
            await memoryConnection.OpenAsync();
            await memoryConnection.ExecuteAsync(
                "CREATE TABLE IF NOT EXISTS Benchmark (" +
                "Id INTEGER, " +
                "Name TEXT, " +
                "PRIMARY KEY (Id))");

            if (IsCreated.Value)
            {
                var count = await connectionFactory.UsingAsync(con =>
                    con.ExecuteScalarAsync<long>("SELECT COUNT(*) FROM Benchmark"));
                IsInserted.Value = count > 0;
            }

            base.OnNavigatingTo(context);
        }

        private async Task Create()
        {
            await connectionFactory.UsingAsync(async con =>
            {
                await con.ExecuteAsync(
                    "CREATE TABLE IF NOT EXISTS Test (" +
                    "Id INTEGER, " +
                    "StringValue TEXT, " +
                    "IntValue INTEGER, " +
                    "LongValue INTEGER, " +
                    "DoubleValue REAL, " +
                    "DecimalValue REAL, " +
                    "BoolValue INTEGER, " +
                    "DateTimeOffsetValue INTEGER, " +
                    "PRIMARY KEY (Id))");
                await con.ExecuteAsync(
                    "CREATE TABLE IF NOT EXISTS Benchmark (" +
                    "Id INTEGER, " +
                    "Name TEXT, " +
                    "PRIMARY KEY (Id))");
            });

            IsCreated.Value = true;
            IsInserted.Value = false;
        }

        private void Drop()
        {
            File.Delete(settings.DatabasePath);

            IsCreated.Value = false;
            IsInserted.Value = false;
        }

        private async Task Insert()
        {
            var ret = await connectionFactory.UsingAsync(async con =>
            {
                try
                {
                    await con.ExecuteAsync(
                        //SqlInsert<TestEntity>.Values(),
                        "INSERT INTO Test (" +
                        "Id, StringValue, IntValue, LongValue, DoubleValue, DecimalValue, BoolValue, DateTimeOffsetValue" +
                        ") VALUES (" +
                        "@Id, @StringValue, @IntValue, @LongValue, @DoubleValue, @DecimalValue, @BoolValue, @DateTimeOffsetValue" +
                        ")",
                        new TestEntity
                        {
                            Id = 1,
                            StringValue = "Test",
                            IntValue = 2,
                            LongValue = 3L,
                            DoubleValue = 4.5d,
                            DecimalValue = 6.7m,
                            BoolValue = true,
                            DateTimeOffsetValue = DateTimeOffset.Now
                        });
                    return true;
                }
                catch (SqliteException e)
                {
                    if (e.SqliteErrorCode == SQLitePCL.raw.SQLITE_CONSTRAINT)
                    {
                        return false;
                    }
                    throw;
                }
            });

            if (ret)
            {
                await dialogs.Information("Inserted");
            }
            else
            {
                await dialogs.Information("Key duplicate");
            }
        }

        private async Task Update()
        {
            var effect = await connectionFactory.UsingAsync(con =>
                con.ExecuteAsync(
                    //SqlUpdate<TestEntity>.Set("StringValue = @StringValue", "Id = @Id"),
                    "UPDATE Test SET StringValue = @StringValue WHERE Id = @Id",
                    new { Id = 1, StringValue = "Updated" }));

            await dialogs.Information($"Effect={effect}");
        }

        private async Task Delete()
        {
            var effect = await connectionFactory.UsingAsync(con =>
                con.ExecuteAsync(
                    //SqlDelete<TestEntity>.ByKey(),
                    "DELETE FROM Test WHERE Id = @Id",
                    new { Id = 1 }));

            await dialogs.Information($"Effect={effect}");
        }

        private async Task Count()
        {
            var count = await connectionFactory.UsingAsync(con =>
                con.ExecuteScalarAsync<long>(
                    //SqlCount<TestEntity>.All()));
                    "SELECT COUNT(*) FROM Test"));

            await dialogs.Information($"Count={count}");
        }

        private async Task Select1()
        {
            var entity = await connectionFactory.UsingAsync(con =>
                con.QueryFirstOrDefaultAsync<TestEntity>(
                    //SqlSelect<TestEntity>.ByKey(),
                    "SELECT * FROM Test WHERE Id = @Id",
                    new { Id = 1 }));

            if (entity != null)
            {
                await dialogs.Information($"StringValue={entity.StringValue}");
            }
            else
            {
                await dialogs.Information("Not found");
            }
        }

        private async Task SelectAll()
        {
            var watch = Stopwatch.StartNew();

            // TODO
            var list = (await connectionFactory.UsingAsync(async con =>
                (await con.QueryAsync<TestEntity>(
                    //SqlSelect<TestEntity>.All())).ToList()));
                    "SELECT * FROM Test ORDER BY Id")).ToList()));

            await dialogs.Information($"Count={list.Count}\r\nElapsed={watch.ElapsedMilliseconds}");
        }

        private async Task InsertBulk()
        {
            var watch = Stopwatch.StartNew();

            await connectionFactory.UsingTxAsync(async (con, tx) =>
            {
                for (var i = 1; i <= 10000; i++)
                {
                    await con.ExecuteAsync(
                        "INSERT INTO Benchmark (Id, Name) VALUES (@Id, @Name)",
                        new BenchmarkEntity { Id = i, Name = $"Name-{i}" },
                        tx);
                }

                tx.Commit();
            });

            await dialogs.Information($"Inserted\r\nElapsed={watch.ElapsedMilliseconds}");

            IsInserted.Value = true;
        }

        private async Task DeleteAll()
        {
            await connectionFactory.UsingAsync(con =>
                con.ExecuteAsync("DELETE FROM Benchmark"));

            IsInserted.Value = false;
        }

        private async Task MemoryInsertBulk()
        {
            var watch = Stopwatch.StartNew();

            using (var tx = memoryConnection.BeginTransaction())
            {
                for (var i = 1; i <= 10000; i++)
                {
                    await memoryConnection.ExecuteAsync(
                        "INSERT INTO Benchmark (Id, Name) VALUES (@Id, @Name)",
                        new BenchmarkEntity { Id = i, Name = $"Name-{i}" },
                        tx);
                }

                tx.Commit();
            }

            await dialogs.Information($"Inserted\r\nElapsed={watch.ElapsedMilliseconds}");

            IsMemoryInserted.Value = true;
        }

        private async Task MemoryDeleteAll()
        {
            await memoryConnection.ExecuteAsync("DELETE FROM Benchmark");

            IsMemoryInserted.Value = false;
        }
    }
}
