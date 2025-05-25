using ApiServerControllerBase.DTOs;
using AutoMapper;
using RepositoryBase.Models;

namespace ApiServerControllerBase.Config;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<TodoItem, TodoItemDto>().ReverseMap();
        CreateMap<CreateTodoItemDto, TodoItem>();
        CreateMap<UpdateTodoItemDto, TodoItem>();
    }
}
