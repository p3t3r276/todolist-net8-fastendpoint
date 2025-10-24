using System.Linq.Expressions;
using FastTodo.Infrastructure.Domain.Entities;

namespace FastTodo.Infrastructure.Domain.Repositories.Builder;

public interface IQueryBuilder<TEntity> : IOrderBuilder<TEntity> where TEntity : class, IEntity
{
    IQueryBuilder<TEntity, TProperty, TProperty> Include<TProperty>(Expression<Func<TEntity, TProperty?>> navigationProperty)
        where TProperty : class, IEntity;

    IQueryBuilder<TEntity, IEnumerable<TProperty>, TProperty> Include<TProperty>(Expression<Func<TEntity, IEnumerable<TProperty>>> navigationProperty);

    IQueryBuilder<TEntity> IgnoreFilter();

    IQueryBuilder<TEntity> Include(string navigationPropertyPath);

    IQueryBuilder<TEntity> AsSplitQuery();
}

public interface IQueryBuilder<TEntity, TProperty, TGeneric> : IQueryBuilder<TEntity>
    where TEntity : class, IEntity
{
    IQueryBuilder<TEntity, TNextProperty, TNextProperty> ThenInclude<TNextProperty>(Expression<Func<TGeneric, TNextProperty?>> navigationProperty) where TNextProperty : class, IEntity;
    IQueryBuilder<TEntity, IEnumerable<TNextProperty>, TNextProperty> ThenInclude<TNextProperty>(Expression<Func<TGeneric, IEnumerable<TNextProperty>>> navigationProperty);
}

public interface IOrderBuilder<TEntity> where TEntity : class, IEntity
{
    IQueryBuilder<TEntity> Order<TProperty>(IComparer<TEntity>? comparer = default);
    IQueryBuilder<TEntity> OrderDescending<TProperty>(IComparer<TEntity>? comparer = default);

    IQueryBuilder<TEntity> OrderBy<TProperty>(Expression<Func<TEntity, TProperty>> keySelector);
    IQueryBuilder<TEntity> OrderByDescending<TProperty>(Expression<Func<TEntity, TProperty>> keySelector);

    IQueryBuilder<TEntity> OrderByName(string propertyName, bool isDescending);
}
