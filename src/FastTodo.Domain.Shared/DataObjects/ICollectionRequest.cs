namespace FastTodo.Domain.Shared;

public interface ICollectionRequest
{
    public int PageIndex { get; set; }

    public int PageSize { get; set; }
}
