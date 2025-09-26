namespace FastTodo.Domain.Shared;

public struct PaginatedList<T>(List<T> items, int count, int pageIndex, int pageSize)
{
    public List<T> Data { get; } = items;

    public int PageIndex { get; } = pageIndex;

    public int PageSize { get; set; } = pageSize;

    public int TotalPages { get; } = (int)Math.Ceiling(count / (double)pageSize);

    public int TotalCount { get; } = count;

    public readonly bool HasPreviousPage => PageIndex > 0;

    public readonly bool HasNextPage => PageIndex < TotalPages;
}
