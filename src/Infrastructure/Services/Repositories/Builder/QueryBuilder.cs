using System.Linq.Expressions;
using FastTodo.Infrastructure.Domain.Entities;
using FastTodo.Infrastructure.Domain.Repositories.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace FastTodo.Infrastructure.Repositories.Builder;

public class QueryBuilder<TEntity>(
    IQueryable<TEntity> query,
    IOrderedQueryable<TEntity>? order = default,
    QueryBuilder<TEntity>? root = default
    ) : IQueryBuilder<TEntity> where TEntity : class, IEntity
{
    private IQueryable<TEntity> _query = query;

    protected IOrderedQueryable<TEntity>? _order = order;

    protected readonly QueryBuilder<TEntity>? _root = root;

    internal IOrderedQueryable<TEntity>? QueryOrder => _order;

    internal virtual IQueryable<TEntity> Query
    {
        get { return _query; }
        set { _query = value; }
    }

    internal void UpdateQuery(IQueryable<TEntity> updateQuery)
    {
        _query = updateQuery;
    }

    public IQueryBuilder<TEntity> AsSplitQuery()
    {
        _query = _query.AsSplitQuery();

        _root?.UpdateQuery(_query);
        return this;
    }

    public IQueryBuilder<TEntity> IgnoreFilter()
    {
        _query = _query.IgnoreQueryFilters();
        _root?.UpdateQuery(_query);
        return this;
    }

    public IQueryBuilder<TEntity, IEnumerable<TProperty>, TProperty> Include<TProperty>(
        Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty)
    {
        _query = _query.Include(navigationProperty);
        _root?.UpdateQuery(_query);
        var builder = new QueryBuilder<TEntity, IEnumerable<TProperty>, TProperty>((_root ?? this).Query, (_root ?? this).QueryOrder, _root ?? this);
        return builder;
    }

    public IQueryBuilder<TEntity> Include(string navigationPropertyPath)
    {
        _query = _query.Include(navigationPropertyPath);
        _root?.UpdateQuery(_query);
        return this;
    }

    public virtual IQueryBuilder<TEntity, TProperty, TProperty> Include<TProperty>(
        Expression<Func<TEntity, TProperty?>> navigationProperty
    ) where TProperty : class, IEntity
    {
        query = _query.Include(navigationProperty);
        _root?.UpdateQuery(_query);
        var builder = new QueryBuilder<TEntity, TProperty, TProperty>((_root ?? this).Query, (_root ?? this).QueryOrder, _root ?? this);
        return builder;
    }

    public IQueryBuilder<TEntity> Order<TProperty>(IComparer<TEntity>? comparer = null)
    {
        _query = comparer is not null ? _query.Order(comparer) : _query.Order();
        _root?.UpdateQuery(_query);
        return this;
    }

    public IQueryBuilder<TEntity> OrderBy<TProperty>(Expression<Func<TEntity, TProperty>> keySelector)
    {
        _order = _order is null ? _query.OrderBy(keySelector) : _order.ThenBy(keySelector);
        _query = _order;

        _root?.UpdateQuery(_query);
        return this;
    }

    public IQueryBuilder<TEntity> OrderDescending<TProperty>(IComparer<TEntity>? comparer = null)
    {
        _query = comparer is not null ? _query.OrderDescending(comparer) : _query.OrderDescending();

        _root?.UpdateQuery(_query);
        return this;
    }

    public IQueryBuilder<TEntity> OrderByDescending<TProperty>(Expression<Func<TEntity, TProperty>> keySelector)
    {
        _order = _order is null ? _query.OrderByDescending(keySelector) : _order.ThenByDescending(keySelector);
        _query = _order;

        _root?.UpdateQuery(_query);
        return this;
    }

    public IQueryBuilder<TEntity> OrderByName(string propertyName, bool isDescending)
    {
        _query = _order is null ?  _query.OrderByName(propertyName, isDescending) : _query.ThenOrderByName(propertyName, isDescending);
        _order = _query as IOrderedQueryable<TEntity>;

        _root?.UpdateQuery(_query);
        return this;
    }
}

internal class QueryBuilder<TEntity, TProperty, TGeneric>(
    IQueryable<TEntity> query,
    IOrderedQueryable<TEntity>? order = default,
    QueryBuilder<TEntity>? root = default
    ) : QueryBuilder<TEntity>(query, order, root), IQueryBuilder<TEntity, TProperty, TGeneric>
    where TEntity : class, IEntity
{
    public IQueryBuilder<TEntity, IEnumerable<TNextProperty>, TNextProperty> ThenInclude<TNextProperty>(
        Expression<Func<TGeneric, IEnumerable<TNextProperty>>> navigationProperty
        )
    {
        if (_root?.Query is IIncludableQueryable<TEntity, TGeneric> includeQueryable)
        {
            var newQuery = includeQueryable.ThenInclude(navigationProperty);
            _root?.UpdateQuery(newQuery);
            return new QueryBuilder<TEntity, IEnumerable<TNextProperty>, TNextProperty>(_root.Query, _root.QueryOrder, _root);
        }

        if (_root?.Query is IIncludableQueryable<TEntity, IEnumerable<TGeneric>> collectionIncludeQueryable)
        {
            var newQuery = collectionIncludeQueryable.ThenInclude(navigationProperty);
            _root?.UpdateQuery(newQuery);
            return new QueryBuilder<TEntity, IEnumerable<TNextProperty>, TNextProperty>(_root.Query, _root.QueryOrder, _root);
        }
        
        throw new InvalidOperationException("Theninclude do not support type");
    }

    public IQueryBuilder<TEntity, TNextProperty, TNextProperty> ThenInclude<TNextProperty>(
        Expression<Func<TGeneric, TNextProperty>> navigationProperty
    ) where TNextProperty : class, IEntity
    {
        if (_root?.Query is IIncludableQueryable<TEntity, TGeneric> singleInclude)
        {
            var newQuery = singleInclude.ThenInclude(navigationProperty);
            _root?.UpdateQuery(newQuery);
            return new QueryBuilder<TEntity, TNextProperty, TNextProperty>(_root.Query, _order, _root);
        }

        if (_root?.Query is IIncludableQueryable<TEntity, IEnumerable<TGeneric>> collectionInclude)
        {
            var newQuery = collectionInclude.ThenInclude(navigationProperty);
            _root?.UpdateQuery(newQuery);
            return new QueryBuilder<TEntity, TNextProperty, TNextProperty>(_root.Query, _order, _root);
        }

        throw new InvalidOperationException("Theninclude do not support type");
    }
}
