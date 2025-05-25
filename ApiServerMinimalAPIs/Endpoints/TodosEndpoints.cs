using ApiServerMinimalAPIs.DTOs;
using ApiServerMinimalAPIs.DTOs.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using RepositoryBase;
using RepositoryBase.Models;

namespace ApiServerMinimalAPIs.Endpoints;

public static class TodosEndpoints
{
    public static RouteGroupBuilder MapTodosEndpoint(this RouteGroupBuilder routeGroup)
    {
        routeGroup.MapGet("/", GetAll)
            .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(30)).Tag("tag-get-todos"))
            .WithSummary("모든 아이템을 조회한다.");

        routeGroup.MapGet("/{id:int}", GetById)
            .WithName("get-by-id")
            .WithSummary("특정 ID에 해당하는 아이템을 조회한다.");

        routeGroup.MapPost("/", Create)
            .AddEndpointFilter<ValidationFilter<CreateTodoItemDto>>()
            .WithSummary("새로운 아이템을 작성한다.");

        routeGroup.MapPut("/{id:int}", UpdateById)
            .AddEndpointFilter<ValidationFilter<UpdateTodoItemDto>>()
            .WithSummary("특정 ID에 해당하는 아이템을 수정한다.");

        routeGroup.MapDelete("/{id:int}", DeleteById)
            .WithSummary("특정 ID에 해당하는 아이템을 삭제한다.");

        routeGroup.MapDelete("/", Clear)
            .WithSummary("모든 아이템을 삭제한다.");

        return routeGroup;
    }

    private static async Task<Ok<List<TodoItemDto>>> GetAll(IRepository<TodoItem> repo, IMapper mapper)
    {
        var todoItems = await repo.GetAllAsync();
        var todoItemDtoList = mapper.Map<List<TodoItemDto>>(todoItems);
        return TypedResults.Ok(todoItemDtoList);
    }

    private static async Task<Results<Ok<TodoItemDto>, NotFound>> GetById(int id, IRepository<TodoItem> repo, IMapper mapper)
    {
        var todoItem = await repo.GetByIdAsync(id);
        if (todoItem is null)
        {
            return TypedResults.NotFound();
        }

        var todoItemDto = mapper.Map<TodoItemDto>(todoItem);
        return TypedResults.Ok(todoItemDto);
    }

    private static async Task<Results<CreatedAtRoute<TodoItemDto>, InternalServerError<ErrorDto>>> Create(CreateTodoItemDto createDto, IRepository<TodoItem> repo, IMapper mapper, IOutputCacheStore outputCacheStore)
    {
        try
        {
            var todoItem = mapper.Map<TodoItem>(createDto);
            todoItem = await repo.CreateAsync(todoItem);

            var todoItemDto = mapper.Map<TodoItemDto>(todoItem);
            await outputCacheStore.EvictByTagAsync("tag-get-todos", default);
            return TypedResults.CreatedAtRoute(todoItemDto, "get-by-id", new { id = todoItemDto.Id });
        }
        catch (Exception ex)
        {
            await ErrorReporter.NotifyExceptionAsync(ex);
            return TypedResults.InternalServerError(new ErrorDto
            {
                Message = "서버 오류가 발생했습니다."
            });
        }
    }

    private static async Task<Results<Ok<TodoItemDto>, NotFound, InternalServerError<ErrorDto>>> UpdateById(int id, UpdateTodoItemDto updateDto, IRepository<TodoItem> repo, IMapper mapper, IOutputCacheStore outputCacheStore)
    {
        try
        {
            var todoItem = await repo.GetByIdAsync(id);
            if (todoItem is null)
            {
                return TypedResults.NotFound();
            }

            if (todoItem.IsCompleted != updateDto.IsCompleted)
            {
                todoItem.IsCompleted = updateDto.IsCompleted;
                todoItem.CompletedAt = updateDto.IsCompleted ? todoItem.CompletedAt = DateTime.Now : null;
            }

            await repo.UpdateAsync(todoItem);
            await outputCacheStore.EvictByTagAsync("tag-get-todos", default);
            var todoItemDto = mapper.Map<TodoItemDto>(todoItem);
            return TypedResults.Ok(todoItemDto);
        }
        catch (Exception ex)
        {
            await ErrorReporter.NotifyExceptionAsync(ex);
            return TypedResults.InternalServerError(new ErrorDto
            {
                Message = "서버 오류가 발생했습니다."
            });
        }
    }

    private static async Task<Results<NoContent, NotFound, InternalServerError<ErrorDto>>> DeleteById(int id, IRepository<TodoItem> repo, IOutputCacheStore outputCacheStore)
    {
        try
        {
            var todoItem = await repo.GetByIdAsync(id);
            if (todoItem is null)
            {
                return TypedResults.NotFound();
            }

            await repo.RemoveAsync(todoItem);
            await outputCacheStore.EvictByTagAsync("tag-get-todos", default);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            await ErrorReporter.NotifyExceptionAsync(ex);
            return TypedResults.InternalServerError(new ErrorDto
            {
                Message = "서버 오류가 발생했습니다."
            });
        }
    }

    private static async Task<Results<NoContent, InternalServerError<ErrorDto>>> Clear(IRepository<TodoItem> repo, IOutputCacheStore outputCacheStore)
    {
        try
        {
            await repo.RemoveAll();
            await outputCacheStore.EvictByTagAsync("tag-get-todos", default);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            await ErrorReporter.NotifyExceptionAsync(ex);
            return TypedResults.InternalServerError(new ErrorDto
            {
                Message = "서버 오류가 발생했습니다."
            });
        }
    }
}

