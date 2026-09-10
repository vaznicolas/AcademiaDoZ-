// Nicolas Vaz

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Email
{
    public string Valor { get; }

    private Email(string valor)
    {
        Valor = valor;
    }

    public static Result<Email> Criar(string valor)
    {
        if (NormalizacaoService.TextoVazioOuNulo(valor))
            return Result<Email>.Failure(
                "Email",
                "EMAIL_OBRIGATORIO");

        var textoLimpo = NormalizacaoService.LimparEspacos(valor);

        if (!EmailValido(textoLimpo))
            return Result<Email>.Failure(
                "Email",
                "EMAIL_INVALIDO");

        return Result<Email>.Success(
            new Email(textoLimpo));
    }

    private static bool EmailValido(string email)
    {
        var partes = email.Split('@');

        if (partes.Length != 2)
            return false;

        if (string.IsNullOrWhiteSpace(partes[0]))
            return false;

        if (string.IsNullOrWhiteSpace(partes[1]))
            return false;

        if (!partes[1].Contains('.'))
            return false;

        return true;
    }

    public override string ToString() => Valor;
}