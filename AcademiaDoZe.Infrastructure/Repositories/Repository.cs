// Nicolas Vaz

using AcademiaDoZe.Domain.Common;
using AcademiaDoZe.Domain.Entities;
using AcademiaDoZe.Domain.Repositories;

namespace AcademiaDoZe.Infrastructure.Repositories;

public class Repository<TEntity> : IRepository<TEntity>
    where TEntity : Entity, IAggregateRoot
{
    protected readonly List<TEntity> _entities = [];

    public Task<TEntity?> ObterPorId(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = _entities.FirstOrDefault(x => x.Id == id);

        return Task.FromResult(entity);
    }

    public Task<IEnumerable<TEntity>> ObterTodos(
        CancellationToken cancellationToken = default)
    {
        IEnumerable<TEntity> entities = _entities.ToList();

        return Task.FromResult(entities);
    }

    public Task<TEntity> Adicionar(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        _entities.Add(entity);

        return Task.FromResult(entity);
    }

    public Task<TEntity> Atualizar(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        var indice = _entities.FindIndex(x => x.Id == entity.Id);

        if (indice >= 0)
            _entities[indice] = entity;

        return Task.FromResult(entity);
    }

    public Task<bool> Remover(
        int id,
        CancellationToken cancellationToken = default)
    {
        var entity = _entities.FirstOrDefault(x => x.Id == id);

        if (entity is null)
            return Task.FromResult(false);

        _entities.Remove(entity);

        return Task.FromResult(true);
    }
}