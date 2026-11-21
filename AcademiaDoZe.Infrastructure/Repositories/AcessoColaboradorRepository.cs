
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AcessoColaboradorRepository
    : BaseRepository, IAcessoColaboradorRepository
{
    private readonly IColaboradorRepository _colaboradorRepository;

    public AcessoColaboradorRepository(
        string connectionString,
        DatabaseType databaseType,
        IColaboradorRepository colaboradorRepository)
        : base(connectionString, databaseType)
    {
        _colaboradorRepository = colaboradorRepository;
    }

    private async Task<List<AcessoColaborador>> ConsultarAsync(
        string sql,
        Action<DbCommand>? parametros = null,
        CancellationToken ct = default)
    {
        await using var command =
            await CreateCommandAsync(sql, ct);

        parametros?.Invoke(command);

        var registros =
            new List<(int Id, int ColaboradorId,
                DateTime Entrada, DateTime? Saida)>();

        await using (var reader =
            await command.ExecuteReaderAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                registros.Add((
                    Convert.ToInt32(reader["id_acesso"]),
                    Convert.ToInt32(reader["pessoa_id"]),
                    Convert.ToDateTime(reader["data_hora"]),
                    reader["data_hora_saida"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(
                            reader["data_hora_saida"])
                ));
            }
        }

        var resultado = new List<AcessoColaborador>();

        foreach (var registro in registros)
        {
            var colaborador =
                await _colaboradorRepository.ObterPorId(
                    registro.ColaboradorId, ct);

            if (colaborador == null)
                throw new InvalidOperationException(
                    $"Colaborador {registro.ColaboradorId} não encontrado.");

            resultado.Add(AcessoColaborador.Criar(
                registro.Id,
                colaborador,
                registro.Entrada,
                registro.Saida));
        }

        return resultado;
    }

    public async Task<AcessoColaborador?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        var lista = await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 1 AND id_acesso = @Id",
            cmd => cmd.AddParameter(
                "@Id", id, DbType.Int32),
            cancellationToken);

        return lista.FirstOrDefault();
    }

    public async Task<IEnumerable<AcessoColaborador>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        return await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 1 " +
            "ORDER BY data_hora DESC",
            ct: cancellationToken);
    }

    public async Task<AcessoColaborador> Adicionar(
        AcessoColaborador entity,
        CancellationToken cancellationToken = default)
    {
        string sql = FormatInsertQuery(
            "INSERT INTO tb_acesso " +
            "(pessoa_tipo, pessoa_id, data_hora, data_hora_saida) " +
            "VALUES (1, @PessoaId, @Entrada, @Saida)");

        await using var command =
            await CreateCommandAsync(sql, cancellationToken);

        command.AddParameter(
            "@PessoaId",
            entity.Colaborador.Id,
            DbType.Int32);

        command.AddParameter(
            "@Entrada",
            entity.DataHoraEntrada,
            DbType.DateTime);

        command.AddParameter(
            "@Saida",
            (object?)entity.DataHoraSaida ?? DBNull.Value,
            DbType.DateTime);

        int id = await command.ExecuteScalarIdAsync(
            "ERRO_ADICIONAR_ACESSO_COLABORADOR",
            "Não foi possível obter o ID do acesso.",
            cancellationToken);

        return AcessoColaborador.Criar(
            id,
            entity.Colaborador,
            entity.DataHoraEntrada,
            entity.DataHoraSaida);
    }

    public async Task<AcessoColaborador> Atualizar(
        AcessoColaborador entity,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "UPDATE tb_acesso SET " +
            "data_hora_saida = @Saida " +
            "WHERE id_acesso = @Id AND pessoa_tipo = 1";

        await using var command =
            await CreateCommandAsync(sql, cancellationToken);

        command.AddParameter(
            "@Id", entity.Id, DbType.Int32);

        command.AddParameter(
            "@Saida",
            (object?)entity.DataHoraSaida ?? DBNull.Value,
            DbType.DateTime);

        int linhas = await command.ExecuteNonQueryAsync(
            cancellationToken);

        if (linhas == 0)
            throw new InvalidOperationException(
                "Acesso do colaborador não encontrado.");

        return entity;
    }

    public async Task<bool> Remover(
        int id,
        CancellationToken cancellationToken = default)
    {
        await using var command =
            await CreateCommandAsync(
                "DELETE FROM tb_acesso " +
                "WHERE id_acesso = @Id AND pessoa_tipo = 1",
                cancellationToken);

        command.AddParameter(
            "@Id", id, DbType.Int32);

        return await command.ExecuteNonQueryAsync(
            cancellationToken) > 0;
    }

    public async Task<IEnumerable<AcessoColaborador>> ObterPorColaborador(
        int colaboradorId,
        CancellationToken cancellationToken = default)
    {
        return await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 1 AND pessoa_id = @Id " +
            "ORDER BY data_hora DESC",
            cmd => cmd.AddParameter(
                "@Id", colaboradorId, DbType.Int32),
            cancellationToken);
    }

    public async Task<IEnumerable<AcessoColaborador>> ObterPorPeriodo(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default)
    {
        return await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 1 " +
            "AND data_hora >= @Inicio " +
            "AND data_hora <= @Fim " +
            "ORDER BY data_hora",
            cmd =>
            {
                cmd.AddParameter(
                    "@Inicio", inicio, DbType.DateTime);
                cmd.AddParameter(
                    "@Fim", fim, DbType.DateTime);
            },
            cancellationToken);
    }

    public async Task<AcessoColaborador?> ObterAberto(
        int colaboradorId,
        CancellationToken cancellationToken = default)
    {
        var lista = await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 1 " +
            "AND pessoa_id = @Id " +
            "AND data_hora_saida IS NULL " +
            "ORDER BY data_hora DESC",
            cmd => cmd.AddParameter(
                "@Id", colaboradorId, DbType.Int32),
            cancellationToken);

        return lista.FirstOrDefault();
    }

    public async Task<AcessoColaborador?> ObterPorData(
        int colaboradorId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        var lista = await ObterTodosPorData(
            colaboradorId, data, cancellationToken);

        return lista.FirstOrDefault();
    }

    public async Task<IEnumerable<AcessoColaborador>> ObterTodosPorData(
        int colaboradorId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        return await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 1 " +
            "AND pessoa_id = @Id " +
            "AND data_hora >= @Inicio " +
            "AND data_hora < @Fim " +
            "ORDER BY data_hora",
            cmd =>
            {
                cmd.AddParameter(
                    "@Id", colaboradorId, DbType.Int32);
                cmd.AddParameter(
                    "@Inicio", data.Date, DbType.DateTime);
                cmd.AddParameter(
                    "@Fim", data.Date.AddDays(1),
                    DbType.DateTime);
            },
            cancellationToken);
    }
}
