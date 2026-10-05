// Nicolas Vaz

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Arquivo
{
    public string Valor { get; }

    private Arquivo(string valor)
    {
        Valor = valor;
    }

    public static Result<Arquivo> Criar(string valor)
    {
        if (NormalizacaoService.TextoVazioOuNulo(valor))
            return Result<Arquivo>.Failure(
                "Arquivo",
                "ARQUIVO_OBRIGATORIO");

        return Result<Arquivo>.Success(new Arquivo(valor));
    }

    public override string ToString() => Valor;
}