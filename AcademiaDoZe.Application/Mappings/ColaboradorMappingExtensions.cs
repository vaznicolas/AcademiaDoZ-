// Nicolas Vaz

using System.Text;
using AcademiaDoZe.Application.DTOs;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Application.Mappings;

public static class ColaboradorMappingExtensions
{
    public static ColaboradorDto ToDto(
        this Colaborador colaborador,
        Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(colaborador);

        return new ColaboradorDto
        {
            Id = colaborador.Id,
            Nome = colaborador.Nome,
            Cpf = colaborador.Cpf.Valor,
            DataNascimento = colaborador.DataNascimento,
            Telefone = colaborador.Telefone.Valor,
            Email = colaborador.Email?.Valor,
            Endereco = logradouro?.ToDto(),
            Numero = colaborador.Endereco.Numero,
            Complemento = colaborador.Endereco.Complemento,
            Senha = null,
            Foto = colaborador.Foto != null
                ? new ArquivoDto
                {
                    Conteudo = Encoding.UTF8.GetBytes(
                        colaborador.Foto.Valor)
                }
                : null,
            DataAdmissao = colaborador.DataAdmissao,
            Tipo = colaborador.Tipo.ToApplication(),
            Vinculo = colaborador.Vinculo.ToApplication()
        };
    }

    public static Colaborador ToEntity(
        this ColaboradorDto colaboradorDto,
        Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(colaboradorDto);

        var logradouroEntidade =
            logradouro
            ?? (colaboradorDto.Endereco != null
                ? colaboradorDto.Endereco.ToEntity()
                : null)
            ?? throw new InvalidOperationException(
                "Logradouro/Endereço é obrigatório para converter o Colaborador.");

        var cpfResult = Cpf.Criar(colaboradorDto.Cpf);
        var telefoneResult = Telefone.Criar(colaboradorDto.Telefone);
        var emailResult = Email.Criar(
            colaboradorDto.Email ?? string.Empty);

        if (cpfResult.IsFailure ||
            telefoneResult.IsFailure ||
            emailResult.IsFailure)
        {
            throw new InvalidOperationException(
                "Erro de validação nos dados do Colaborador.");
        }

        var enderecoResult = Endereco.Criar(
            logradouroEntidade,
            colaboradorDto.Numero,
            colaboradorDto.Complemento ?? string.Empty);

        if (enderecoResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação do Endereço: " +
                $"{string.Join(", ", enderecoResult.Notifications.Select(n => n.Mensagem))}");
        }

        var senhaResult = Senha.Criar(
            colaboradorDto.Senha ?? string.Empty);

        if (senhaResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação da Senha: " +
                $"{string.Join(", ", senhaResult.Notifications.Select(n => n.Mensagem))}");
        }

        Arquivo? foto = null;

        if (colaboradorDto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(
                Encoding.UTF8.GetString(
                    colaboradorDto.Foto.Conteudo));

            if (fotoResult.IsSuccess)
                foto = fotoResult.Value;
        }

        return Colaborador.Criar(
            colaboradorDto.Id,
            colaboradorDto.Nome,
            cpfResult.Value!,
            colaboradorDto.DataNascimento,
            telefoneResult.Value!,
            emailResult.Value!,
            enderecoResult.Value!,
            senhaResult.Value!,
            foto!,
            colaboradorDto.DataAdmissao,
            colaboradorDto.Tipo.ToDomain(),
            colaboradorDto.Vinculo.ToDomain());
    }

    public static Colaborador UpdateFromDto(
        this Colaborador colaborador,
        ColaboradorDto colaboradorDto,
        Logradouro? logradouro = null)
    {
        ArgumentNullException.ThrowIfNull(colaborador);
        ArgumentNullException.ThrowIfNull(colaboradorDto);

        var logradouroEntidade =
            logradouro
            ?? (colaboradorDto.Endereco != null
                ? colaboradorDto.Endereco.ToEntity()
                : null)
            ?? throw new InvalidOperationException(
                "Logradouro/Endereço é obrigatório para atualizar o Colaborador.");

        var cpfResult = Cpf.Criar(colaborador.Cpf.Valor);
        var telefoneResult = Telefone.Criar(
            colaboradorDto.Telefone ?? colaborador.Telefone.Valor);
        var emailResult = Email.Criar(
            colaboradorDto.Email ?? colaborador.Email.Valor);

        if (cpfResult.IsFailure ||
            telefoneResult.IsFailure ||
            emailResult.IsFailure)
        {
            throw new InvalidOperationException(
                "Erro de validação nos dados do Colaborador.");
        }

        var enderecoResult = Endereco.Criar(
            logradouroEntidade,
            colaboradorDto.Numero ?? colaborador.Endereco.Numero,
            colaboradorDto.Complemento ?? colaborador.Endereco.Complemento);

        if (enderecoResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação do Endereço: " +
                $"{string.Join(", ", enderecoResult.Notifications.Select(n => n.Mensagem))}");
        }

        Arquivo? foto = colaborador.Foto;

        if (colaboradorDto.Foto?.Conteudo != null)
        {
            var fotoResult = Arquivo.Criar(
                Encoding.UTF8.GetString(
                    colaboradorDto.Foto.Conteudo));

            if (fotoResult.IsSuccess)
                foto = fotoResult.Value;
        }

        var senhaResult = Senha.Criar(
            !string.IsNullOrWhiteSpace(colaboradorDto.Senha)
                ? colaboradorDto.Senha
                : colaborador.Senha.Valor);

        if (senhaResult.IsFailure)
        {
            throw new InvalidOperationException(
                $"Erro de validação da Senha: " +
                $"{string.Join(", ", senhaResult.Notifications.Select(n => n.Mensagem))}");
        }

        return Colaborador.Criar(
            colaborador.Id,
            colaboradorDto.Nome ?? colaborador.Nome,
            cpfResult.Value!,
            colaboradorDto.DataNascimento != default
                ? colaboradorDto.DataNascimento
                : colaborador.DataNascimento,
            telefoneResult.Value!,
            emailResult.Value!,
            enderecoResult.Value!,
            senhaResult.Value!,
            foto!,
            colaboradorDto.DataAdmissao != default
                ? colaboradorDto.DataAdmissao
                : colaborador.DataAdmissao,
            colaboradorDto.Tipo != default
                ? colaboradorDto.Tipo.ToDomain()
                : colaborador.Tipo,
            colaboradorDto.Vinculo != default
                ? colaboradorDto.Vinculo.ToDomain()
                : colaborador.Vinculo);
    }
}