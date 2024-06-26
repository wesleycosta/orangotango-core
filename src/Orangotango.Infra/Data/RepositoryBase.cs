﻿using Microsoft.EntityFrameworkCore;
using Orangotango.Core.Abstractions;
using Orangotango.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orangotango.Infra.Data;

public abstract class RepositoryBase<TEntity> : IRepositoryBase<TEntity> where TEntity : EntityBase
{
    protected readonly DbSet<TEntity> DbSet;

    protected RepositoryBase(DbContext context)
        => DbSet = context.Set<TEntity>();

    protected virtual IQueryable<TEntity> AsQueryable()
        => DbSet.AsQueryable();

    public virtual void Add(TEntity entity) =>
        DbSet.Add(entity);

    public virtual void Update(TEntity entity) =>
        DbSet.Update(entity);

    public virtual void SoftDelete(TEntity entity)
    {
        entity.Remove();
        Update(entity);
    }

    public virtual void HardDelete(TEntity entity)
        => DbSet.Remove(entity);

    public virtual async Task<TEntity> GetById(Guid id)
        => await AsQueryable().FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<TResult>> GetAll<TResult>(Expression<Func<TEntity, TResult>> projection)
    {
        var query = AsQueryable();

        return await query
            .Select(projection)
            .ToListAsync();
    }

    public async Task<IEnumerable<TResult>> GetByExpression<TResult>(Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> projection)
    {
        var query = AsQueryable();

        return await query
            .Where(filter)
            .Select(projection)
            .ToListAsync();
    }

    public async Task<IEnumerable<TResult>> GetByExpression<TResult, TKey>(
        Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> projection,
        Expression<Func<TEntity, TKey>> orderByExpression,
        bool ascending = true,
        params Expression<Func<TEntity, object>>[] includes)
    {
        var query = AsQueryable();
        foreach (var include in includes)
        {
            query = query.Include(include);
        }

        query = query.Where(filter);
        query = ascending ? query.OrderBy(orderByExpression) : query.OrderByDescending(orderByExpression);

        return await query.Select(projection).ToListAsync();
    }

    public async Task<TResult> GetFirstByExpression<TResult>(Expression<Func<TEntity, bool>> filter,
        Expression<Func<TEntity, TResult>> projection)
    {
        var query = AsQueryable();

        return await query
            .Where(filter)
            .Select(projection)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> Any(Expression<Func<TEntity, bool>> predicate)
        => await AsQueryable().AnyAsync(predicate);
}
