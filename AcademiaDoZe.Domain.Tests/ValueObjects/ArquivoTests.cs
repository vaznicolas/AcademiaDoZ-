// Nicolas Vaz

using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Tests.ValueObjects;

public class ArquivoTests
{
    [Fact]
    public void Deve_Criar_Arquivo_Quando_Valor_For_Valido()
    {
        var resultado = Arquivo.Criar("arquivo.pdf");

        Assert.True(resultado.IsSuccess);
        Assert.Equal("arquivo.pdf", resultado.Value!.Valor);
    }

    [Fact]
    public void Deve_Retornar_Erro_Quando_Arquivo_For_Vazio()
    {
        var resultado = Arquivo.Criar("");

        Assert.False(resultado.IsSuccess);
        Assert.Contains(
            resultado.Notifications,
            notification => notification.Mensagem == "ARQUIVO_OBRIGATORIO");
    }

    [Fact]
    public void Deve_Retornar_Erro_Quando_Arquivo_For_Nulo()
    {
        var resultado = Arquivo.Criar(null!);

        Assert.False(resultado.IsSuccess);
        Assert.Contains(
            resultado.Notifications,
            notification => notification.Mensagem == "ARQUIVO_OBRIGATORIO");
    }

    [Fact]
    public void Deve_Retornar_Valor_Ao_Converter_Arquivo_Para_String()
    {
        var resultado = Arquivo.Criar("arquivo.pdf");

        Assert.Equal("arquivo.pdf", resultado.Value!.ToString());
    }
}