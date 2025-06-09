using System.Linq.Expressions;

namespace FastTodo.Infrastructure.Domain.Repositories.Builder;

public interface IEntitySetter<TEntity> where TEntity : class
{
    IEntitySetter<TEntity> Set<TProperty>(Expression<Func<TEntity, TProperty>> expression, TProperty value);
}
