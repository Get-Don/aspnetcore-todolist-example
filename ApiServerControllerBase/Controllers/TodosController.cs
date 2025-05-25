using ApiServerControllerBase.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RepositoryBase;
using RepositoryBase.Models;
using System.Net;

namespace ApiServerControllerBase.Controllers;

[ApiController]
//[Route("api/[controller]")]
[Route("api/todos")]    // 명시적으로 사용
[Produces("application/json")]
public class TodosController : ControllerBase
{
    private readonly IRepository<TodoItem> _repo;
    private readonly IMapper _mapper;    

    public TodosController(IRepository<TodoItem> repo, IMapper mapper)
    {
        _mapper = mapper;
        _repo = repo;
    }

    //[HttpGet("get-all")]  엔드포인트를 다르게 지정하는 방법
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("모든 아이템을 조회한다.")]
    public async Task<ActionResult<List<TodoItemDto>>> GetAll()
    {
        var todoItems = await _repo.GetAllAsync();
        var todoItemDtoList = _mapper.Map<List<TodoItemDto>>(todoItems);
        return Ok(todoItemDtoList);
    }
        
    [HttpGet("{id:int}", Name = "get-by-id")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("특정 ID에 해당하는 아이템을 조회한다.")]
    public async Task<ActionResult<TodoItemDto>> GetById(int id)
    {
        var todoItem = await _repo.GetByIdAsync(id);
        if(todoItem is null)
        {
            return NotFound();
        }

        var todoItemDto = _mapper.Map<TodoItemDto>(todoItem);
        return Ok(todoItem);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("새로운 아이템을 작성한다.")]
    public async Task<ActionResult<TodoItemDto>> Create([FromBody] CreateTodoItemDto createDto)
    {
        try
        {
            var todoItem = _mapper.Map<TodoItem>(createDto);
            todoItem = await _repo.CreateAsync(todoItem);

            var todoItemDto = _mapper.Map<TodoItemDto>(todoItem);

            return CreatedAtRoute("get-by-id", new { id = todoItemDto.Id }, todoItemDto);
            //return CreatedAtAction(nameof(GetById), new { id = todoItemDto.Id }, todoItemDto);
        }
        catch (Exception ex)
        {
            await ErrorReporter.NotifyExceptionAsync(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorDto
            {
                Message = "서버 오류가 발생했습니다."
            });
        }
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("특정 ID에 해당하는 아이템을 수정한다.")]
    public async Task<ActionResult<TodoItemDto>> UpdateById(int id, [FromBody] UpdateTodoItemDto updateDto)
    {
        try
        {
            var todoItem = await _repo.GetByIdAsync(id);
            if (todoItem is null)
            {
                return NotFound();
            }

            if (todoItem.IsCompleted != updateDto.IsCompleted)
            {
                todoItem.IsCompleted = updateDto.IsCompleted;
                todoItem.CompletedAt = updateDto.IsCompleted ? todoItem.CompletedAt = DateTime.Now : null;
            }

            await _repo.UpdateAsync(todoItem);
            var todoItemDto = _mapper.Map<TodoItemDto>(todoItem);

            return Ok(todoItemDto);
        }
        catch (Exception ex)
        {
            await ErrorReporter.NotifyExceptionAsync(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorDto
            {
                Message = "서버 오류가 발생했습니다."
            });
        }
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("특정 ID에 해당하는 아이템을 삭제한다.")]
    public async Task<IActionResult> DeleteById(int id)
    {
        try
        {
            var todoItem = await _repo.GetByIdAsync(id);
            if (todoItem is null)
            {
                return NotFound();
            }

            await _repo.RemoveAsync(todoItem);
            return NoContent();
        }
        catch (Exception ex)
        {
            await ErrorReporter.NotifyExceptionAsync(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorDto
            {
                Message = "서버 오류가 발생했습니다."
            });
        }
    }

    [HttpDelete]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorDto), StatusCodes.Status500InternalServerError)]
    [EndpointSummary("모든 아이템을 삭제한다.")]
    public async Task<IActionResult> Clear()
    {
        try
        {
            await _repo.RemoveAll();
            return NoContent();
        }
        catch(Exception ex)
        {
            await ErrorReporter.NotifyExceptionAsync(ex);
            return StatusCode((int)HttpStatusCode.InternalServerError, new ErrorDto
            {
                Message = "서버 오류가 발생했습니다."
            });
        }

    }
}
