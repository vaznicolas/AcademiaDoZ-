// Nicolas Vaz

using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Enums;
using AcademiaDoZe.Domain.Repositories;
using AcademiaDoZe.Domain.ValueObjects;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class ColaboradorRepository
    : Repository<Colaborador>, IColaboradorRepository
{
    public Task<Colaborador?> ObterPorCpf(
        Cpf cpf,
        CancellationToken cancellationToken = default)
    {
        var colaborador = _entities
            .FirstOrDefault(x => x.Cpf == cpf);

        return Task.FromResult(colaborador);
    }

    public Task<Colaborador?> ObterPorEmail(
        Email email,
        CancellationToken cancellationToken = default)
    {
        var colaborador = _entities
            .FirstOrDefault(x => x.Email == email);

        return Task.FromResult(colaborador);
    }

    public Task<bool> CpfJaExiste(
        Cpf cpf,
        int? id = null,
        CancellationToken cancellationToken = default)
    {
        var existe = _entities.Any(x =>
            x.Cpf == cpf &&
            (!id.HasValue || x.Id != id.Value));

        return Task.FromResult(existe);
    }

    public Task<bool> EmailJaExiste(
        Email email,
        int? id = null,
        CancellationToken cancellationToken = default)
    {
        var existe = _entities.Any(x =>
            x.Email == email &&
            (!id.HasValue || x.Id != id.Value));

        return Task.FromResult(existe);
    }

    public Task<IEnumerable<Colaborador>> ObterPorTipo(
        ColaboradorTipo tipo,
        CancellationToken cancellationToken = default)
    {
        var colaboradores = _entities
            .Where(x => x.Tipo == tipo)
            .ToList();

        return Task.FromResult<IEnumerable<Colaborador>>(colaboradores);
    }

    public Task<IEnumerable<Colaborador>> ObterPorVinculo(
        ColaboradorVinculo vinculo,
        CancellationToken cancellationToken = default)
    {
        var colaboradores = _entities
            .Where(x => x.Vinculo == vinculo)
            .ToList();

        return Task.FromResult<IEnumerable<Colaborador>>(colaboradores);
    }

    public Task<bool> TrocarSenha(
        int id,
        Senha novaSenha,
        CancellationToken cancellationToken = default)
    {
        var colaborador = _entities
            .FirstOrDefault(x => x.Id == id);

        if (colaborador is null)
            return Task.FromResult(false);

        colaborador.AlterarSenha(novaSenha);

        return Task.FromResult(true);
    }
}