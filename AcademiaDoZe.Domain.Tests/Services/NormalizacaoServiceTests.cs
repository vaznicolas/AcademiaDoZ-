// Nicolas Vaz

using AcademiaDoZe.Domain.Services;

namespace AcademiaDoZe.Tests.Services;

public class NormalizacaoServiceTests
{
    [Fact]
    public void Deve_Identificar_Texto_Vazio()
    {
        var resultado = NormalizacaoService.TextoVazioOuNulo("");

        Assert.True(resultado);
    }

    [Fact]
    public void Deve_Identificar_Texto_Nulo()
    {
        var resultado = NormalizacaoService.TextoVazioOuNulo(null);

        Assert.True(resultado);
    }

    [Fact]
    public void Deve_Identificar_Texto_Com_Conteudo()
    {
        var resultado = NormalizacaoService.TextoVazioOuNulo("Academia");

        Assert.False(resultado);
    }

    [Fact]
    public void Deve_Limpar_Espacos_Excedentes()
    {
        var resultado = NormalizacaoService.LimparEspacos(
            "   Academia    do     Zé   ");

        Assert.Equal("Academia do Zé", resultado);
    }

    [Fact]
    public void Deve_Retornar_String_Vazia_Ao_Limpar_Espacos_De_Texto_Vazio()
    {
        var resultado = NormalizacaoService.LimparEspacos("");

        Assert.Equal(string.Empty, resultado);
    }

    [Fact]
    public void Deve_Remover_Todos_Os_Espacos()
    {
        var resultado = NormalizacaoService.LimparTodosEspacos(
            "Academia do Zé");

        Assert.Equal("AcademiadoZé", resultado);
    }

    [Fact]
    public void Deve_Retornar_String_Vazia_Ao_Remover_Espacos_De_Texto_Vazio()
    {
        var resultado = NormalizacaoService.LimparTodosEspacos("");

        Assert.Equal(string.Empty, resultado);
    }

    [Fact]
    public void Deve_Converter_Texto_Para_Maiusculo()
    {
        var resultado = NormalizacaoService.ParaMaiusculo(
            "Academia do Zé");

        Assert.Equal("ACADEMIA DO ZÉ", resultado);
    }

    [Fact]
    public void Deve_Retornar_String_Vazia_Ao_Converter_Texto_Nulo_Para_Maiusculo()
    {
        var resultado = NormalizacaoService.ParaMaiusculo(null);

        Assert.Equal(string.Empty, resultado);
    }

    [Fact]
    public void Deve_Manter_Apenas_Digitos()
    {
        var resultado = NormalizacaoService.LimparEDigitos(
            "(49) 99999-9999");

        Assert.Equal("49999999999", resultado);
    }

    [Fact]
    public void Deve_Retornar_String_Vazia_Ao_Limpar_Digitos_De_Texto_Vazio()
    {
        var resultado = NormalizacaoService.LimparEDigitos("");

        Assert.Equal(string.Empty, resultado);
    }

    [Fact]
    public void Deve_Identificar_Texto_Com_Apenas_Espacos()
    {
        var resultado = NormalizacaoService.TextoVazioOuNulo("   ");

        Assert.True(resultado);
    }

    [Fact]
    public void Deve_Retornar_String_Vazia_Ao_Limpar_Espacos_De_Texto_Nulo()
    {
        var resultado = NormalizacaoService.LimparEspacos(null);

        Assert.Equal(string.Empty, resultado);
    }

    [Fact]
    public void Deve_Retornar_String_Vazia_Ao_Remover_Todos_Os_Espacos_De_Texto_Nulo()
    {
        var resultado = NormalizacaoService.LimparTodosEspacos(null);

        Assert.Equal(string.Empty, resultado);
    }

    [Fact]
    public void Deve_Retornar_String_Vazia_Ao_Converter_Texto_Vazio_Para_Maiusculo()
    {
        var resultado = NormalizacaoService.ParaMaiusculo("");

        Assert.Equal(string.Empty, resultado);
    }

    [Fact]
    public void Deve_Retornar_Apenas_Digitos_Quando_Texto_Tiver_Letras_E_Simbolos()
    {
        var resultado = NormalizacaoService.LimparEDigitos(
            "ABC-123.XYZ");

        Assert.Equal("123", resultado);
    }

}