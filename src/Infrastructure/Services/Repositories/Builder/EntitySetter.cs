using FastTodo.Infrastructure.Domain.Repositories.Builder;
using System.Linq.Expressions;

namespace FastTodo.Infrastructure.Repositories.Builder;

public class EntitySetter<TEntity>(TEntity entity) : IEntitySetter<TEntity> where TEntity : class
{
    internal HashSet<string> UpdateProperties { get; } = [];

    internal TEntity Item => entity;

    public IEntitySetter<TEntity> Set<TProperty>(Expression<Func<TEntity, TProperty>> expression, TProperty value)
    {
        ArgumentNullException.ThrowIfNull(expression);
        var propertyExpression = (MemberExpression)(expression.Body is UnaryExpression unary ? unary.Operand : expression.Body);
        var propertyName = propertyExpression.Member.Name;

        UpdateProperties.Add(propertyName);

        typeof(TEntity).GetProperty(propertyName)?.SetValue(entity, value);
        return this;
    }
}
