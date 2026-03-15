using FastTodo.Domain.Entities.Mongo;
using FastTodo.Domain.Shared;
using Mapster;

namespace FastTodo.Application.Features.Todo.Models;

public class TodoItemMappingRegister : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<TodoItemSchema, TodoItemDto>()
            .Map(dest => dest.Id, src => src.TaskId);

        config.NewConfig<PaginatedList<TodoItemSchema>, PaginatedList<TodoItemDto>>()
            .Map(dest => dest.Data, src => src.Data.Adapt<List<TodoItemDto>>())
            .Map(dest => dest.PageIndex, src => src.PageIndex)
            .Map(dest => dest.PageSize, src => src.PageSize);
    }
}
