using System.Linq.Expressions;
using FastTodo.Infrastructure.Domain.Entities;

namespace FastTodo.Infrastructure.Domain.Repositories.Builder;

public interface IEntitySetter<TEntity> where TEntity : class, IEntity
{
    IEntitySetter<TEntity> Set<TProperty>(Expression<Func<TEntity, TProperty>> expression, TProperty value);
}
