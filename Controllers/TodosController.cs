using Microsoft.AspNetCore.Mvc;
using MyTextApi.Interfaces;
using MyTextApi.Models.DTOs;

namespace MyTextApi.Controllers;

[ApiController]
[Route("api/todos")]
public class TodosController : ControllerBase
{
    private readonly ITodoService _todoService;

    public TodosController(ITodoService todoService)
    {
        _todoService = todoService;
    }

    // GET /api/todos
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var todos = await _todoService.GetAllTodosAsync();
        return Ok(todos);
    }

    // GET /api/todos/users/5
    [HttpGet("users/{userId}")]
    public async Task<IActionResult> GetByUserId(int userId)
    {
        var todos = await _todoService.GetTodosByUserIdAsync(userId);
        return Ok(todos);
    }

    // GET /api/todos/5
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var todo = await _todoService.GetTodoByIdAsync(id);
        return todo == null
            ? NotFound(new { message = "Todo not found" })
            : Ok(todo);
    }

    // POST /api/todos
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTodoRequest request)
    {
        try
        {
            var todo = await _todoService.CreateTodoAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = todo.Id }, todo);

        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // PUT /api/todos/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTodoRequest request)
    {
        var todo = await _todoService.UpdateTodoAsync(id, request);
        return todo == null
            ? NotFound(new { message = "Todo not found" })
            : Ok(todo);
    }

    // PATCH /api/todos/5/toggle
    [HttpPatch("{id}/toggle")]
    public async Task<IActionResult> Toggle(int id)
    {
        var todo = await _todoService.ToggleTodoAsync(id);
        return todo == null
            ? NotFound(new { message = "Todo not found" })
            : Ok(todo);
    }

    // Delete /api/todos/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _todoService.DeleteTodoAsync(id);
        return deleted
            ? Ok(new { message = $"Todo {id} deleted successfully" })
            : NotFound(new { message = "Todo not found" });
    }

}