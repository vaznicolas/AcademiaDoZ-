// Nicolas Vaz

namespace AcademiaDoZe.Domain.ValueObjects;

public record Arquivo
{
    public string Valor { get; }

    private Arquivo(string valor)
    {
        Valor = valor;
    }
}