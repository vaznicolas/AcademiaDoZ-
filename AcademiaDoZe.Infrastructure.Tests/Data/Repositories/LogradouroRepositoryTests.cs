// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Infrastructure.Tests.Repositories;

public class LogradouroRepositoryTests : TestBase
{
    private readonly LogradouroRepository _repository;

    public LogradouroRepositoryTests()
    {
        _repository =
            new LogradouroRepository(
                ConnectionString,
                DatabaseType);
    }

    [Fact]
    public async Task Deve_Adicionar_E_Obter_Logradouro()
    {
        var resultado = Logradouro.Criar(
            0,
            GerarCep(),
            "Rua Teste",
            "Centro",
            "Florianopolis",
            "SC",
            "Brasil");

        Assert.True(resultado.IsSuccess);

        var logradouro = resultado.Value!;

        var adicionado =
            await _repository.Adicionar(logradouro);

        var encontrado =
            await _repository.ObterPorId(adicionado.Id);

        Assert.NotNull(encontrado);
        Assert.Equal(
            adicionado.Id,
            encontrado.Id);

        Assert.Equal(
            logradouro.Cep.Valor,
            encontrado.Cep.Valor);

        await _repository.Remover(
            adicionado.Id);
    }

    [Fact]
    public async Task Deve_Obter_Todos_Os_Logradouros()
    {
        var resultado = Logradouro.Criar(
            0,
            GerarCep(),
            "Rua Todos",
            "Centro",
            "Florianopolis",
            "SC",
            "Brasil");

        Assert.True(resultado.IsSuccess);

        var adicionado =
            await _repository.Adicionar(
                resultado.Value!);

        var logradouros =
            await _repository.ObterTodos();

        Assert.Contains(
            logradouros,
            x => x.Id == adicionado.Id);

        await _repository.Remover(
            adicionado.Id);
    }

    [Fact]
    public async Task Deve_Atualizar_Logradouro()
    {
        var resultado = Logradouro.Criar(
            0,
            GerarCep(),
            "Rua Original",
            "Centro",
            "Florianopolis",
            "SC",
            "Brasil");

        Assert.True(resultado.IsSuccess);

        var adicionado =
            await _repository.Adicionar(
                resultado.Value!);

        var atualizadoResultado =
            Logradouro.Criar(
                adicionado.Id,
                GerarCep(),
                "Rua Atualizada",
                "Novo Bairro",
                "Sao Jose",
                "SC",
                "Brasil");

        Assert.True(
            atualizadoResultado.IsSuccess);

        var atualizado =
            await _repository.Atualizar(
                atualizadoResultado.Value!);

        var encontrado =
            await _repository.ObterPorId(
                atualizado.Id);

        Assert.NotNull(encontrado);

        Assert.Equal(
            "Rua Atualizada",
            encontrado.Nome);

        Assert.Equal(
            "Novo Bairro",
            encontrado.Bairro);

        Assert.Equal(
            "Sao Jose",
            encontrado.Cidade);

        await _repository.Remover(
            adicionado.Id);
    }

    [Fact]
    public async Task Deve_Remover_Logradouro()
    {
        var resultado = Logradouro.Criar(
            0,
            GerarCep(),
            "Rua Remover",
            "Centro",
            "Florianopolis",
            "SC",
            "Brasil");

        Assert.True(resultado.IsSuccess);

        var adicionado =
            await _repository.Adicionar(
                resultado.Value!);

        var removido =
            await _repository.Remover(
                adicionado.Id);

        var encontrado =
            await _repository.ObterPorId(
                adicionado.Id);

        Assert.True(removido);
        Assert.Null(encontrado);
    }

    [Fact]
    public async Task Deve_Obter_Por_Cep()
    {
        var cepValor =
            GerarCep();

        var resultado =
            Logradouro.Criar(
                0,
                cepValor,
                "Rua CEP",
                "Centro",
                "Florianopolis",
                "SC",
                "Brasil");

        Assert.True(resultado.IsSuccess);

        var adicionado =
            await _repository.Adicionar(
                resultado.Value!);

        var cep =
            adicionado.Cep;

        var encontrado =
            await _repository.ObterPorCep(
                cep);

        Assert.NotNull(encontrado);

        Assert.Equal(
            adicionado.Id,
            encontrado.Id);

        Assert.Equal(
            cep.Valor,
            encontrado.Cep.Valor);

        await _repository.Remover(
            adicionado.Id);
    }

    [Fact]
    public async Task Deve_Verificar_Se_Cep_Ja_Existe()
    {
        var resultado =
            Logradouro.Criar(
                0,
                GerarCep(),
                "Rua CEP Existente",
                "Centro",
                "Florianopolis",
                "SC",
                "Brasil");

        Assert.True(resultado.IsSuccess);

        var adicionado =
            await _repository.Adicionar(
                resultado.Value!);

        var cep =
            adicionado.Cep;

        var existe =
            await _repository.CepJaExiste(
                cep);

        Assert.True(existe);

        await _repository.Remover(
            adicionado.Id);
    }

    [Fact]
    public async Task Deve_Permitir_Mesmo_Cep_Ao_Atualizar_O_Proprio_Logradouro()
    {
        var resultado =
            Logradouro.Criar(
                0,
                GerarCep(),
                "Rua CEP Atualizacao",
                "Centro",
                "Florianopolis",
                "SC",
                "Brasil");

        Assert.True(resultado.IsSuccess);

        var adicionado =
            await _repository.Adicionar(
                resultado.Value!);

        var existe =
            await _repository.CepJaExiste(
                adicionado.Cep,
                adicionado.Id);

        Assert.False(existe);

        await _repository.Remover(
            adicionado.Id);
    }

    [Fact]
    public async Task Deve_Obter_Por_Cidade()
    {
        var cidade = "SQLServer";

        var resultado = Logradouro.Criar(
            0,
            GerarCep(),
            "Nicolas",
            "Vaz",
            cidade,
            "SC",
            "Brasil");

        Assert.True(resultado.IsSuccess);

        var adicionado =
            await _repository.Adicionar(
                resultado.Value!);

        var logradouros =
            await _repository.ObterPorCidade(
                cidade);

        Assert.Contains(
            logradouros,
            x => x.Id == adicionado.Id);

        await _repository.Remover(
            adicionado.Id);
    }

    [Fact]
    public async Task Deve_Obter_Por_Bairro()
    {
        var cidade =
            $"CidadeTeste_{Guid.NewGuid():N}";

        var bairro =
            $"BairroTeste_{Guid.NewGuid():N}";

        var resultado =
            Logradouro.Criar(
                0,
                GerarCep(),
                "Rua Bairro",
                bairro,
                cidade,
                "SC",
                "Brasil");

        Assert.True(resultado.IsSuccess);

        var adicionado =
            await _repository.Adicionar(
                resultado.Value!);

        var logradouros =
            await _repository.ObterPorBairro(
                cidade,
                bairro);

        Assert.Contains(
            logradouros,
            x => x.Id == adicionado.Id);

        await _repository.Remover(
            adicionado.Id);

    }

}