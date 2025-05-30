namespace FastTodo.Infrastructure.Domain.Entities;

public interface IEntity<TKey>
{
    TKey Id { get; set; }
}
