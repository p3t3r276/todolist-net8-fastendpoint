namespace FastTodo.Domain.Shared;

public class PaginatedList<T>
{
    public List<T> Data { get; }

    public int PageIndex { get; }

    public int PageSize { get; set; }

    public int TotalPages { get; }

    public int TotalCount { get; }

    public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
    {
        PageIndex = pageIndex;
        PageSize = pageSize;
        TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        TotalCount = count;
        Data = items;
    }

    public bool HasPreviousPage => PageIndex > 1;

    public bool HasNextPage => PageIndex < TotalPages;
}
