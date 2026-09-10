// Nicolas Vaz

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Senha
{
    public string Valor { get; }

    private Senha(string valor)
    {
        Valor = valor;
    }

    public static Result<Senha> Criar(string valor)
    {
        if (NormalizacaoService.TextoVazioOuNulo(valor))
            return Result<Senha>.Failure(
                "Senha",
                "SENHA_OBRIGATORIO");

        if (valor.Length < 8)
            return Result<Senha>.Failure(
                "Senha",
                "SENHA_TAMANHO");

        if (!valor.Any(char.IsUpper))
            return Result<Senha>.Failure(
                "Senha",
                "SENHA_MAIUSCULA");

        if (!valor.Any(char.IsLower))
            return Result<Senha>.Failure(
                "Senha",
                "SENHA_MINUSCULA");

        if (!valor.Any(char.IsDigit))
            return Result<Senha>.Failure(
                "Senha",
                "SENHA_NUMERO");

        return Result<Senha>.Success(new Senha(valor));
    }

    public override string ToString() => Valor;
}