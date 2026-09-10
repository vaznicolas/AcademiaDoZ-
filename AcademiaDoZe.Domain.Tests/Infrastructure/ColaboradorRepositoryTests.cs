// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.ValueObjects;
using AcademiaDoZe.Infrastructure.Repositories;

namespace AcademiaDoZe.Domain.Tests.Infrastructure;

public class ColaboradorRepositoryTests
{
    private static Colaborador CriarColaborador(
        int id,
        string nome,
        ColaboradorTipo tipo = ColaboradorTipo.Instrutor,
        ColaboradorVinculo vinculo = ColaboradorVinculo.CLT)
    {
        var logradouro = Logradouro.Criar(
            1,
            "88500000",
            "Rua das Flores",
            "Centro",
            "Lages",
            "SC",
            "Brasil").Value!;

        var endereco = Endereco.Criar(
            logradouro,
            "123",
            "Apto 101").Value!;

        return Colaborador.Criar(
            id,
            nome,
            Cpf.Criar("12345678909").Value!,
            new DateOnly(2000, 1, 1),
            Telefone.Criar("(49) 99999-9999").Value!,
            Email.Criar($"colaborador{id}@email.com").Value!,
            endereco,
            Senha.Criar("Senha123").Value!,
            Arquivo.Criar("foto.jpg").Value!,
            new DateOnly(2026, 1, 10),
            tipo,
            vinculo);
    }

    [Fact]
    public async Task Deve_Obter_Colaborador_Por_Cpf()
    {
        var repository = new ColaboradorRepository();

        var colaborador = CriarColaborador(1, "Nicolas Vaz");

        await repository.Adicionar(colaborador);

        var resultado = await repository.ObterPorCpf(colaborador.Cpf);

        Assert.Equal(colaborador, resultado);
    }

    [Fact]
    public async Task Deve_Obter_Colaborador_Por_Email()
    {
        var repository = new ColaboradorRepository();

        var colaborador = CriarColaborador(1, "Nicolas Vaz");

        await repository.Adicionar(colaborador);

        var resultado = await repository.ObterPorEmail(colaborador.Email);

        Assert.Equal(colaborador, resultado);
    }

    [Fact]
    public async Task Deve_Verificar_Se_Cpf_Ja_Existe()
    {
        var repository = new ColaboradorRepository();

        var colaborador = CriarColaborador(1, "Nicolas Vaz");

        await repository.Adicionar(colaborador);

        var resultado = await repository.CpfJaExiste(colaborador.Cpf);

        Assert.True(resultado);
    }

    [Fact]
    public async Task Deve_Verificar_Se_Email_Ja_Existe()
    {
        var repository = new ColaboradorRepository();

        var colaborador = CriarColaborador(1, "Nicolas Vaz");

        await repository.Adicionar(colaborador);

        var resultado = await repository.EmailJaExiste(colaborador.Email);

        Assert.True(resultado);
    }

    [Fact]
    public async Task Deve_Obter_Colaboradores_Por_Tipo()
    {
        var repository = new ColaboradorRepository();

        var instrutor = CriarColaborador(
            1,
            "Nicolas Vaz",
            ColaboradorTipo.Instrutor);

        var atendente = CriarColaborador(
            2,
            "Joao Silva",
            ColaboradorTipo.Atendente);

        await repository.Adicionar(instrutor);
        await repository.Adicionar(atendente);

        var resultado = await repository.ObterPorTipo(
            ColaboradorTipo.Instrutor);

        Assert.Single(resultado);
        Assert.Equal("Nicolas Vaz", resultado.First().Nome);
    }

    [Fact]
    public async Task Deve_Obter_Colaboradores_Por_Vinculo()
    {
        var repository = new ColaboradorRepository();

        var clt = CriarColaborador(
            1,
            "Nicolas Vaz",
            ColaboradorTipo.Instrutor,
            ColaboradorVinculo.CLT);

        var estagio = CriarColaborador(
            2,
            "Joao Silva",
            ColaboradorTipo.Instrutor,
            ColaboradorVinculo.Estagio);

        await repository.Adicionar(clt);
        await repository.Adicionar(estagio);

        var resultado = await repository.ObterPorVinculo(
            ColaboradorVinculo.CLT);

        Assert.Single(resultado);
        Assert.Equal("Nicolas Vaz", resultado.First().Nome);
    }

    [Fact]
    public async Task Deve_Trocar_Senha_Do_Colaborador()
    {
        var repository = new ColaboradorRepository();

        var colaborador = CriarColaborador(1, "Nicolas Vaz");

        await repository.Adicionar(colaborador);

        var novaSenha = Senha.Criar("NovaSenha123").Value!;

        var resultado = await repository.TrocarSenha(
            colaborador.Id,
            novaSenha);

        Assert.True(resultado);
        Assert.Equal(novaSenha, colaborador.Senha);
    }

    [Fact]
    public async Task Deve_Retornar_Falso_Ao_Trocar_Senha_De_Colaborador_Inexistente()
    {
        var repository = new ColaboradorRepository();

        var novaSenha = Senha.Criar("NovaSenha123").Value!;

        var resultado = await repository.TrocarSenha(
            999,
            novaSenha);

        Assert.False(resultado);
    }
}