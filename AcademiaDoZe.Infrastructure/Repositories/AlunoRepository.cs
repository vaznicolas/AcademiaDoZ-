// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Data;
using AcademiaDoZe.Infrastructure.Exceptions;
using System.Data;
using System.Data.Common;
using System.Text;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class AlunoRepository : BaseRepository, IAlunoRepository
{
    public AlunoRepository(
        string connectionString,
        DatabaseType databaseType)
        : base(connectionString, databaseType)
    {
    }

    private static string BaseSelectQuery =>
        "SELECT " +
        "a.id_aluno, a.cpf, a.nome, a.nascimento, a.telefone, a.email, " +
        "a.logradouro_id, a.numero, a.complemento, a.senha, a.foto, " +
        "l.id_logradouro, l.cep, l.nome AS logradouro_nome, l.bairro, l.cidade, l.estado, l.pais " +
        "FROM tb_aluno a " +
        "INNER JOIN tb_logradouro l ON a.logradouro_id = l.id_logradouro";

    public async Task<Aluno?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string query =
                $"{BaseSelectQuery} WHERE a.id_aluno = @Id";

            await using var command =
                await CreateCommandAsync(query, cancellationToken);

            command.AddParameter("@Id", id, DbType.Int32);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            return await reader.ReadAsync(cancellationToken)
                ? Map(reader)
                : null;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_OBTER_POR_ID",
                $"Erro ao obter aluno por ID {id}: {ex.Message}",
                ex);
        }
    }

    public async Task<IEnumerable<Aluno>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        try
        {
            string query =
                $"{BaseSelectQuery} ORDER BY a.nome";

            await using var command =
                await CreateCommandAsync(query, cancellationToken);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            var alunos = new List<Aluno>();

            while (await reader.ReadAsync(cancellationToken))
            {
                alunos.Add(Map(reader));
            }

            return alunos;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_OBTER_TODOS",
                $"Erro ao obter todos os alunos: {ex.Message}",
                ex);
        }
    }

    public static Aluno Map(
        DbDataReader reader,
        string nomeColumn = "nome")
    {
        try
        {
            int id =
                reader.GetInt32Value("id_aluno");

            var cpf =
                Cpf.Criar(reader.GetStringValue("cpf")).Value!;

            string nome =
                reader.GetStringValue(nomeColumn);

            DateOnly nascimento =
                reader.GetDateOnlyValue("nascimento");

            var telefone =
                Telefone.Criar(reader.GetStringValue("telefone")).Value!;

            var email =
                Email.Criar(reader.GetStringValue("email")).Value!;

            string numero =
                reader.GetStringValue("numero");

            string complemento =
                reader.GetNullableString("complemento");

            var senha =
                Senha.Criar(reader.GetStringValue("senha")).Value!;

            byte[]? fotoBytes =
                reader.GetNullableBytes("foto");

            var foto =
                Arquivo.Criar(
                    fotoBytes != null
                        ? Encoding.UTF8.GetString(fotoBytes)
                        : "sem-foto").Value!;

            var logradouro =
                LogradouroRepository.Map(reader, "logradouro_nome");

            var endereco =
                Endereco.Criar(logradouro, numero, complemento).Value!;

            return Aluno.Criar(
                id,
                nome,
                cpf,
                nascimento,
                telefone,
                email,
                endereco,
                senha,
                foto);
        }
        catch (Exception ex)
            when (ex is not InfrastructureException)
        {
            throw new InfrastructureException(
                "ERRO_MAPEAMENTO_ALUNO",
                $"Erro ao mapear dados do aluno: {ex.Message}",
                ex);
        }
    }

    public async Task<Aluno> Adicionar(
        Aluno entity,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string query =
                FormatInsertQuery(
                    "INSERT INTO tb_aluno " +
                    "(cpf, nome, nascimento, telefone, email, logradouro_id, numero, " +
                    "complemento, senha, foto) " +
                    "VALUES " +
                    "(@Cpf, @Nome, @Nascimento, @Telefone, @Email, @LogradouroId, @Numero, " +
                    "@Complemento, @Senha, @Foto)");

            await using var command =
                await CreateCommandAsync(query, cancellationToken);

            command.AddParameter("@Cpf", entity.Cpf.Valor, DbType.String);
            command.AddParameter("@Nome", entity.Nome, DbType.String);
            command.AddParameter("@Nascimento", entity.DataNascimento.ToDateTime(TimeOnly.MinValue), DbType.Date);
            command.AddParameter("@Telefone", entity.Telefone.Valor, DbType.String);
            command.AddParameter("@Email", entity.Email.Valor, DbType.String);
            command.AddParameter("@LogradouroId", entity.Endereco.LogradouroId, DbType.Int32);
            command.AddParameter("@Numero", entity.Endereco.Numero, DbType.String);
            command.AddParameter("@Complemento", (object?)entity.Endereco.Complemento ?? DBNull.Value, DbType.String);
            command.AddParameter("@Senha", entity.Senha.Valor, DbType.String);
            command.AddParameter(
                "@Foto",
                entity.Foto is null
                    ? DBNull.Value
                    : Encoding.UTF8.GetBytes(entity.Foto.Valor),
                DbType.Binary);

            int id =
                await command.ExecuteScalarIdAsync(
                    "ERRO_ADICIONAR_ALUNO",
                    "Falha ao obter ID inserido para o aluno.",
                    cancellationToken);

            var idProperty =
                typeof(Entity).GetProperty("Id");

            idProperty?.SetValue(entity, id);

            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_ADICIONAR_ALUNO",
                $"Erro ao adicionar aluno: {ex.Message}",
                ex);
        }
    }

    public async Task<Aluno> Atualizar(
        Aluno entity,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string query =
                "UPDATE tb_aluno SET " +
                "cpf = @Cpf, nome = @Nome, nascimento = @Nascimento, telefone = @Telefone, " +
                "email = @Email, logradouro_id = @LogradouroId, numero = @Numero, " +
                "complemento = @Complemento, senha = @Senha, foto = @Foto " +
                "WHERE id_aluno = @Id";

            await using var command =
                await CreateCommandAsync(query, cancellationToken);

            command.AddParameter("@Id", entity.Id, DbType.Int32);
            command.AddParameter("@Cpf", entity.Cpf.Valor, DbType.String);
            command.AddParameter("@Nome", entity.Nome, DbType.String);
            command.AddParameter("@Nascimento", entity.DataNascimento.ToDateTime(TimeOnly.MinValue), DbType.Date);
            command.AddParameter("@Telefone", entity.Telefone.Valor, DbType.String);
            command.AddParameter("@Email", entity.Email.Valor, DbType.String);
            command.AddParameter("@LogradouroId", entity.Endereco.LogradouroId, DbType.Int32);
            command.AddParameter("@Numero", entity.Endereco.Numero, DbType.String);
            command.AddParameter("@Complemento", (object?)entity.Endereco.Complemento ?? DBNull.Value, DbType.String);
            command.AddParameter("@Senha", entity.Senha.Valor, DbType.String);
            command.AddParameter(
                "@Foto",
                entity.Foto is null
                    ? DBNull.Value
                    : Encoding.UTF8.GetBytes(entity.Foto.Valor),
                DbType.Binary);

            int rowsAffected =
                await command.ExecuteNonQueryAsync(cancellationToken);

            if (rowsAffected == 0)
            {
                throw new InfrastructureException(
                    "REGISTRO_NAO_ENCONTRADO",
                    $"Nenhum aluno encontrado com ID {entity.Id} para atualização.");
            }

            return entity;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_ATUALIZAR_ALUNO",
                $"Erro ao atualizar aluno ID {entity.Id}: {ex.Message}",
                ex);
        }
    }

    public async Task<bool> Remover(
        int id,
        CancellationToken cancellationToken = default)
    {
        try
        {
            string query =
                "DELETE FROM tb_aluno WHERE id_aluno = @Id";

            await using var command =
                await CreateCommandAsync(query, cancellationToken);

            command.AddParameter("@Id", id, DbType.Int32);

            var result =
                await command.ExecuteNonQueryAsync(cancellationToken);

            return result > 0;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_REMOVER_ALUNO",
                $"Erro ao remover aluno ID {id}: {ex.Message}",
                ex);
        }
    }

    public async Task<IEnumerable<Aluno>> ObterPorNome(
        string nome,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Evitamos LOWER()/UPPER() diretamente na coluna para não impedir o uso de índices
            // (full table scan). A collation padrão do SQL Server já é case-insensitive.
            string query =
                $"{BaseSelectQuery} WHERE a.nome LIKE @Nome ORDER BY a.nome";

            await using var command =
                await CreateCommandAsync(query, cancellationToken);

            command.AddParameter("@Nome", $"%{nome}%", DbType.String);

            await using var reader =
                await command.ExecuteReaderAsync(cancellationToken);

            var alunos = new List<Aluno>();

            while (await reader.ReadAsync(cancellationToken))
            {
                alunos.Add(Map(reader));
            }

            return alunos;
        }
        catch (DbException ex)
        {
            throw new InfrastructureException(
                "ERRO_OBTER_POR_NOME",
                $"Erro ao obter alunos por nome {nome}: {ex.Message}",
                ex);
        }
    }
}