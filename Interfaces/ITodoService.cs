using MyTextApi.Models.DTOs;

namespace MyTextApi.Interfaces;

public interface ITodoService
{
    Task<List<TodoResponse>> GetAllTodosAsync();
    Task<List<TodoResponse>> GetTodosByUserIdAsync(int userId);
    Task<TodoResponse?> GetTodoByIdAsync(int id);
    Task<TodoResponse> CreateTodoAsync(int userId, CreateTodoRequest request);
    Task<TodoResponse?> UpdateTodoAsync(int id, UpdateTodoRequest request);
    Task<TodoResponse?> ToggleTodoAsync(int id);
    Task<bool> DeleteTodoAsync(int id);
}