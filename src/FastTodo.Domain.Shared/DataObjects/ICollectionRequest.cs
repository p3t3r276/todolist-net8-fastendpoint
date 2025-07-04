using System.ComponentModel;

namespace FastTodo.Domain.Shared;

public interface ICollectionRequest
{
    [DefaultValue(1)]
    public int PageIndex { get; set; }

    [DefaultValue(10)]
    public int PageSize { get; set; }
}
