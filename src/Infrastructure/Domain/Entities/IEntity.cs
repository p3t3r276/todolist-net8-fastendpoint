namespace FastTodo.Infrastructure.Domain.Entities;

public interface IEntity { }

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}
