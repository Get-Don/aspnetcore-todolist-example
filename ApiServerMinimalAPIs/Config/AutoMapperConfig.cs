using ApiServerMinimalAPIs.DTOs;
using AutoMapper;
using RepositoryBase.Models;

namespace ApiServerMinimalAPIs.Config;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<TodoItem, TodoItemDto>().ReverseMap();
        CreateMap<CreateTodoItemDto, TodoItem>();
        CreateMap<UpdateTodoItemDto, TodoItem>();
    }
}
