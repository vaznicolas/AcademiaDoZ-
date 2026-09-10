// Nicolas Vaz

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Domain.ValueObjects;

public record Cpf
{
    public string Valor { get; }

    private Cpf(string valor)
    {
        Valor = valor;
    }

    public static Result<Cpf> Criar(string valor)
    {
        if (NormalizacaoService.TextoVazioOuNulo(valor))
            return Result<Cpf>.Failure("Cpf", "CPF_OBRIGATORIO");

        var textoLimpo = NormalizacaoService.LimparEDigitos(valor);

        if (textoLimpo.Length != 11)
            return Result<Cpf>.Failure("Cpf", "CPF_DIGITOS");

        if (!CpfValido(textoLimpo))
            return Result<Cpf>.Failure("Cpf", "CPF_INVALIDO");

        return Result<Cpf>.Success(new Cpf(textoLimpo));
    }

    private static bool CpfValido(string cpf)
    {
        if (cpf.Distinct().Count() == 1)
            return false;

        var primeiroDigito = CalcularDigito(cpf[..9]);
        var segundoDigito = CalcularDigito(cpf[..10]);

        return cpf[9] == primeiroDigito &&
               cpf[10] == segundoDigito;
    }

    private static char CalcularDigito(string cpfParcial)
    {
        var soma = 0;
        var peso = cpfParcial.Length + 1;

        foreach (var digito in cpfParcial)
        {
            soma += (digito - '0') * peso;
            peso--;
        }

        var resto = soma % 11;
        var resultado = 11 - resto;

        return resultado >= 10
            ? '0'
            : (char)('0' + resultado);
    }

    public override string ToString() => Valor;
}