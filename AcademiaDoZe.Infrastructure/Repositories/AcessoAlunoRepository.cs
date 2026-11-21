
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Infrastructure.Data;
using System.Data;
using System.Data.Common;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AcessoAlunoRepository
    : BaseRepository, IAcessoAlunoRepository
{
    private readonly IAlunoRepository _alunoRepository;

    public AcessoAlunoRepository(
        string connectionString,
        DatabaseType databaseType,
        IAlunoRepository alunoRepository)
        : base(connectionString, databaseType)
    {
        _alunoRepository = alunoRepository;
    }

    private async Task<AcessoAluno> MapAsync(
        DbDataReader reader,
        CancellationToken ct)
    {
        int id = Convert.ToInt32(reader["id_acesso"]);
        int alunoId = Convert.ToInt32(reader["pessoa_id"]);

        DateTime entrada =
            Convert.ToDateTime(reader["data_hora"]);

        DateTime? saida =
            reader["data_hora_saida"] == DBNull.Value
                ? null
                : Convert.ToDateTime(reader["data_hora_saida"]);

        var aluno = await _alunoRepository.ObterPorId(
            alunoId, ct);

        if (aluno == null)
            throw new InvalidOperationException(
                $"Aluno {alunoId} não encontrado.");

        return AcessoAluno.Criar(id, aluno, entrada, saida);
    }

    private async Task<List<AcessoAluno>> ConsultarAsync(
        string sql,
        Action<DbCommand>? parametros = null,
        CancellationToken ct = default)
    {
        await using var command =
            await CreateCommandAsync(sql, ct);

        parametros?.Invoke(command);

        var registros =
            new List<(int Id, int AlunoId,
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

        var resultado = new List<AcessoAluno>();

        foreach (var registro in registros)
        {
            var aluno = await _alunoRepository.ObterPorId(
                registro.AlunoId, ct);

            if (aluno == null)
                throw new InvalidOperationException(
                    $"Aluno {registro.AlunoId} não encontrado.");

            resultado.Add(AcessoAluno.Criar(
                registro.Id,
                aluno,
                registro.Entrada,
                registro.Saida));
        }

        return resultado;
    }

    public async Task<AcessoAluno?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        var lista = await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 0 AND id_acesso = @Id",
            cmd => cmd.AddParameter(
                "@Id", id, DbType.Int32),
            cancellationToken);

        return lista.FirstOrDefault();
    }

    public async Task<IEnumerable<AcessoAluno>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        return await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 0 " +
            "ORDER BY data_hora DESC",
            ct: cancellationToken);
    }

    public async Task<AcessoAluno> Adicionar(
        AcessoAluno entity,
        CancellationToken cancellationToken = default)
    {
        string sql = FormatInsertQuery(
            "INSERT INTO tb_acesso " +
            "(pessoa_tipo, pessoa_id, data_hora, data_hora_saida) " +
            "VALUES (0, @PessoaId, @Entrada, @Saida)");

        await using var command =
            await CreateCommandAsync(sql, cancellationToken);

        command.AddParameter(
            "@PessoaId",
            entity.Aluno.Id,
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
            "ERRO_ADICIONAR_ACESSO_ALUNO",
            "Não foi possível obter o ID do acesso.",
            cancellationToken);

        return AcessoAluno.Criar(
            id,
            entity.Aluno,
            entity.DataHoraEntrada,
            entity.DataHoraSaida);
    }

    public async Task<AcessoAluno> Atualizar(
        AcessoAluno entity,
        CancellationToken cancellationToken = default)
    {
        const string sql =
            "UPDATE tb_acesso SET " +
            "data_hora_saida = @Saida " +
            "WHERE id_acesso = @Id AND pessoa_tipo = 0";

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
                "Acesso do aluno não encontrado.");

        return entity;
    }

    public async Task<bool> Remover(
        int id,
        CancellationToken cancellationToken = default)
    {
        await using var command =
            await CreateCommandAsync(
                "DELETE FROM tb_acesso " +
                "WHERE id_acesso = @Id AND pessoa_tipo = 0",
                cancellationToken);

        command.AddParameter(
            "@Id", id, DbType.Int32);

        return await command.ExecuteNonQueryAsync(
            cancellationToken) > 0;
    }

    public async Task<IEnumerable<AcessoAluno>> ObterPorAluno(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        return await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 0 AND pessoa_id = @Id " +
            "ORDER BY data_hora DESC",
            cmd => cmd.AddParameter(
                "@Id", alunoId, DbType.Int32),
            cancellationToken);
    }

    public async Task<IEnumerable<AcessoAluno>> ObterPorPeriodo(
        DateTime inicio,
        DateTime fim,
        CancellationToken cancellationToken = default)
    {
        return await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 0 " +
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

    public async Task<AcessoAluno?> ObterAberto(
        int alunoId,
        CancellationToken cancellationToken = default)
    {
        var lista = await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 0 " +
            "AND pessoa_id = @Id " +
            "AND data_hora_saida IS NULL " +
            "ORDER BY data_hora DESC",
            cmd => cmd.AddParameter(
                "@Id", alunoId, DbType.Int32),
            cancellationToken);

        return lista.FirstOrDefault();
    }

    public async Task<AcessoAluno?> ObterPorData(
        int alunoId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        var lista = await ObterTodosPorData(
            alunoId, data, cancellationToken);

        return lista.FirstOrDefault();
    }

    public async Task<IEnumerable<AcessoAluno>> ObterTodosPorData(
        int alunoId,
        DateTime data,
        CancellationToken cancellationToken = default)
    {
        return await ConsultarAsync(
            "SELECT * FROM tb_acesso " +
            "WHERE pessoa_tipo = 0 " +
            "AND pessoa_id = @Id " +
            "AND data_hora >= @Inicio " +
            "AND data_hora < @Fim " +
            "ORDER BY data_hora",
            cmd =>
            {
                cmd.AddParameter(
                    "@Id", alunoId, DbType.Int32);
                cmd.AddParameter(
                    "@Inicio", data.Date, DbType.DateTime);
                cmd.AddParameter(
                    "@Fim", data.Date.AddDays(1),
                    DbType.DateTime);
            },
            cancellationToken);
    }
}
