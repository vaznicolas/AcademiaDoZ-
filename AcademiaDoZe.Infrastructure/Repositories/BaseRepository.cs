// Nicolas Vaz

using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public abstract class BaseRepository
{
    protected readonly string _connectionString;
    protected readonly DatabaseType _databaseType;

    protected BaseRepository(
        string connectionString,
        DatabaseType databaseType)
    {
        _connectionString =
            connectionString
            ?? throw new InfrastructureException(
                "STRING_CONEXAO_NULA",
                $"String de conexão não pode ser nula: {nameof(connectionString)}");

        _databaseType = databaseType;
    }

    protected virtual async Task<DbConnection> GetOpenConnectionAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            await DbInitializer.InicializarAsync(
                _connectionString,
                _databaseType,
                CancellationToken.None);

            var connection =
                DbProvider.CreateConnection(
                    _connectionString,
                    _databaseType);

            try
            {
                await connection.OpenAsync(cancellationToken);

                return connection;
            }
            catch
            {
                await connection.DisposeAsync();
                throw;
            }
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "FALHA_ABRIR_CONEXAO",
                "Falha ao abrir conexão com o banco de dados.",
                ex);
        }
    }

    protected virtual async Task<DbCommand> CreateCommandAsync(
        string commandText,
        CancellationToken cancellationToken = default)
    {
        var connection =
            await GetOpenConnectionAsync(cancellationToken);

        try
        {
            var command =
                DbProvider.CreateCommand(
                    commandText,
                    connection);

            return new ConnectionOwnedCommand(
                command,
                connection);
        }
        catch
        {
            await connection.DisposeAsync();
            throw;
        }
    }

    protected string FormatInsertQuery(
        string insertSql) =>
        DbProvider.FormatInsertQuery(
            insertSql,
            _databaseType);

    protected string GetCurrentDateFunction() =>
        DbProvider.GetCurrentDateFunction(
            _databaseType);

    protected string GetDateAddDaysExpression(
        string dateExpr,
        string daysParam) =>
        DbProvider.GetDateAddDaysExpression(
            dateExpr,
            daysParam,
            _databaseType);

    protected string GetDateHourExpression(
        string dateColumn) =>
        DbProvider.GetDateHourExpression(
            dateColumn,
            _databaseType);

    protected string GetDateMonthExpression(
        string dateColumn) =>
        DbProvider.GetDateMonthExpression(
            dateColumn,
            _databaseType);

    protected string GetDateDayExpression(
        string dateColumn) =>
        DbProvider.GetDateDayExpression(
            dateColumn,
            _databaseType);

    private sealed class ConnectionOwnedCommand : DbCommand
    {
        private readonly DbCommand _command;
        private readonly DbConnection _connection;

        public ConnectionOwnedCommand(
            DbCommand command,
            DbConnection connection)
        {
            _command = command;
            _connection = connection;
        }

        public override string CommandText
        {
            get => _command.CommandText;
            set => _command.CommandText = value;
        }

        public override int CommandTimeout
        {
            get => _command.CommandTimeout;
            set => _command.CommandTimeout = value;
        }

        public override CommandType CommandType
        {
            get => _command.CommandType;
            set => _command.CommandType = value;
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get => _command.UpdatedRowSource;
            set => _command.UpdatedRowSource = value;
        }

        protected override DbConnection? DbConnection
        {
            get => _command.Connection;
            set => _command.Connection = value;
        }

        protected override DbParameterCollection DbParameterCollection =>
            _command.Parameters;

        protected override DbTransaction? DbTransaction
        {
            get => _command.Transaction;
            set => _command.Transaction = value;
        }

        public override bool DesignTimeVisible
        {
            get => _command.DesignTimeVisible;
            set => _command.DesignTimeVisible = value;
        }

        protected override DbParameter CreateDbParameter() =>
            _command.CreateParameter();

        protected override DbDataReader ExecuteDbDataReader(
            CommandBehavior behavior) =>
            _command.ExecuteReader(behavior);

        public override int ExecuteNonQuery() =>
            _command.ExecuteNonQuery();

        public override object? ExecuteScalar() =>
            _command.ExecuteScalar();

        public override void Cancel()
        {
            _command.Cancel();
        }

        public override void Prepare()
        {
            _command.Prepare();
        }

        public override Task<int> ExecuteNonQueryAsync(
            CancellationToken cancellationToken) =>
            _command.ExecuteNonQueryAsync(cancellationToken);

        public override Task<object?> ExecuteScalarAsync(
            CancellationToken cancellationToken) =>
            _command.ExecuteScalarAsync(cancellationToken);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _command.Dispose();
                _connection.Dispose();
            }

            base.Dispose(disposing);
        }

        public override async ValueTask DisposeAsync()
        {
            await _command.DisposeAsync();
            await _connection.DisposeAsync();

            GC.SuppressFinalize(this);
        }
    }
}