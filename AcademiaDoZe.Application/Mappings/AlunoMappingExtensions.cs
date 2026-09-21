// Nicolas Vaz

using System.Text;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class AlunoMappingExtensions
{
    public static AlunoDto ToDto(
        this Aluno aluno,
        Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(aluno);

        return new AlunoDto
        {
            Id = aluno.Id,
            Nome = aluno.Nome,
            Cpf = aluno.Cpf.Valor,
            DataNascimento = aluno.DataNascimento,
            Telefone = aluno.Telefone.Valor,
            Email = aluno.Email?.Valor,
            Endereco = logradouro?.ToDto(),
            Numero = aluno.Endereco.Numero,
            Complemento = aluno.Endereco.Complemento,
            Senha = null,
            Foto = aluno.Foto != null
                ? new ArquivoDto
                {
                    Conteudo = Encoding.UTF8.GetBytes(aluno.Foto.Valor)
                }
                : null
        };
    }

    public static Aluno ToEntity(
        this AlunoDto alunoDto,
        Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(alunoDto);

        var logradouroEntidade =
            logradouro
            ?? (alunoDto.Endereco != null
                ? alunoDto.Endereco.ToEntity()
                : null)
            ?? throw new InvalidOperationException(
                "Logradouro/Endereço é obrigatório para converter o Aluno.");

        var cpfResult = Cpf.Criar(alunoDto.Cpf);
        if (cpfResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação do CPF: " +
                $"{string.Join(", ", cpfResult.Notifications.Select(n => n.Mensagem))}");
        }

        var telefoneResult = Telefone.Criar(alunoDto.Telefone);
        if (telefoneResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação do telefone: " +
                $"{string.Join(", ", telefoneResult.Notifications.Select(n => n.Mensagem))}");
        }

        var emailResult = Email.Criar(alunoDto.Email ?? string.Empty);
        if (emailResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação do Email: " +
                $"{string.Join(", ", emailResult.Notifications.Select(n => n.Mensagem))}");
        }

        var enderecoResult = Endereco.Criar(
            logradouroEntidade,
            alunoDto.Numero,
            alunoDto.Complemento ?? string.Empty);

        if (enderecoResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação do Endereço: " +
                $"{string.Join(", ", enderecoResult.Notifications.Select(n => n.Mensagem))}");
        }

        var senhaResult = Senha.Criar(alunoDto.Senha ?? string.Empty);
        if (senhaResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação da Senha: " +
                $"{string.Join(", ", senhaResult.Notifications.Select(n => n.Mensagem))}");
        }

        Arquivo? foto = null;

        if (alunoDto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(
                Encoding.UTF8.GetString(alunoDto.Foto.Conteudo));

            if (fotoResult.IsSuccess)
                foto = fotoResult.Value;
        }

        var result = Aluno.Criar(
            alunoDto.Id,
            alunoDto.Nome,
            cpfResult.Value!,
            alunoDto.DataNascimento,
            telefoneResult.Value!,
            emailResult.Value!,
            enderecoResult.Value!,
            senhaResult.Value!,
            foto!);

        return result;
    }

    public static Aluno UpdateFromDto(
        this Aluno aluno,
        AlunoDto alunoDto,
        Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(aluno);
        ArgumentNullException.ThrowIfNull(alunoDto);

        var logradouroEntidade =
            logradouro
            ?? (alunoDto.Endereco != null
                ? alunoDto.Endereco.ToEntity()
                : null)
            ?? throw new InvalidOperationException(
                "Logradouro/Endereço é obrigatório para atualizar o Aluno.");

        var cpfResult = Cpf.Criar(aluno.Cpf.Valor);
        var telefoneResult = Telefone.Criar(
            alunoDto.Telefone ?? aluno.Telefone.Valor);
        var emailResult = Email.Criar(
            alunoDto.Email ?? aluno.Email.Valor);

        if (telefoneResult.IsFailure || emailResult.IsFailure)
        {
            throw new InvalidOperationException(
                "Erro de validação ao atualizar os dados do Aluno.");
        }

        var enderecoResult = Endereco.Criar(
            logradouroEntidade,
            alunoDto.Numero ?? aluno.Endereco.Numero,
            alunoDto.Complemento ?? aluno.Endereco.Complemento);

        if (enderecoResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação do Endereço: " +
                $"{string.Join(", ", enderecoResult.Notifications.Select(n => n.Mensagem))}");
        }

        Arquivo? foto = aluno.Foto;

        if (alunoDto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(
                Encoding.UTF8.GetString(alunoDto.Foto.Conteudo));

            if (fotoResult.IsSuccess)
                foto = fotoResult.Value;
        }

        var senhaResult = Senha.Criar(
            !string.IsNullOrWhiteSpace(alunoDto.Senha)
                ? alunoDto.Senha
                : aluno.Senha.Valor);

        if (senhaResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação da Senha: " +
                $"{string.Join(", ", senhaResult.Notifications.Select(n => n.Mensagem))}");
        }

        var result = Aluno.Criar(
            aluno.Id,
            alunoDto.Nome ?? aluno.Nome,
            cpfResult.Value!,
            alunoDto.DataNascimento != default
                ? alunoDto.DataNascimento
                : aluno.DataNascimento,
            telefoneResult.Value!,
            emailResult.Value!,
            enderecoResult.Value!,
            senhaResult.Value!,
            foto!);

        return result;
    }
}