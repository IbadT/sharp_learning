using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using MyTextApi.Data;
using MyTextApi.Interfaces;
using MyTextApi.Models.DTOs;
using MyTextApi.Models.Entities;

namespace MyTextApi.Services;

public class TodoService : ITodoService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public TodoService(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<TodoResponse>> GetAllTodosAsync()
    {
        const string cacheKey = "todos:all";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<List<TodoResponse>>(cached)!;
        }

        var todos = await _context.Todos
            .Include(t => t.User)
            .Select(t => MapToResponse(t))
            .ToListAsync();

        await SaveToCache(cacheKey, todos);
        return todos;
    }

    public async Task<List<TodoResponse>> GetTodosByUserIdAsync(int userId)
    {
        var cacheKey = $"todos:user:{userId}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<List<TodoResponse>>(cached)!;
        }

        var todos = await _context.Todos
            .Include(t => t.User)
            .Where(t => t.UserId == userId)
            .Select(t => MapToResponse(t))
            .ToListAsync();

        await SaveToCache(cacheKey, todos);
        return todos;
    }

    public async Task<TodoResponse?> GetTodoByIdAsync(int id)
    {
        var cacheKey = $"todos:{id}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<TodoResponse>(cached)!;
        }

        var todo = await _context.Todos
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (todo == null) return null;

        var response = MapToResponse(todo);
        await SaveToCache(cacheKey, response);
        return response;
    }

    public async Task<TodoResponse> CreateTodoAsync(CreateTodoRequest request)
    {
        var userExists = await _context.Users
            .AnyAsync(u => u.Id == request.UserId);

        if (!userExists)
        {
            throw new InvalidOperationException($"User {request.UserId} not found");
        }

        var todo = new Todo
        {
            Title = request.Title,
            Description = request.Description ?? string.Empty,
            UserId = request.UserId,
            IsCompleted = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Todos.Add(todo);
        await _context.SaveChangesAsync();

        // Инвалидируем кэш
        await InvalidateCache(request.UserId);

        // Загружаем User для ответа
        await _context.Entry(todo).Reference(t => t.User).LoadAsync();

        return MapToResponse(todo);
    }

    public async Task<TodoResponse?> UpdateTodoAsync(int id, UpdateTodoRequest request)
    {
        var todo = await _context.Todos
            .FindAsync(id);

        if (todo == null) return null;

        todo.Title = request.Title;
        todo.Description = request.Description ?? string.Empty;
        todo.IsCompleted = request.IsCompleted;
        todo.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await InvalidateCache(todo.UserId, todo.Id);

        await _context.Entry(todo)
            .Reference(t => t.User)
            .LoadAsync();

        return MapToResponse(todo);
    }

    public async Task<TodoResponse?> ToggleTodoAsync(int id)
    {
        var todo = await _context.Todos
            .FindAsync(id);

        if (todo == null) return null;

        todo.IsCompleted = !todo.IsCompleted;
        todo.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        await InvalidateCache(todo.UserId, id);
        await _context.Entry(todo)
            .Reference(t => t.User)
            .LoadAsync();

        return MapToResponse(todo);
    }

    public async Task<bool> DeleteTodoAsync(int id)
    {
        var todo = await _context.Todos
            .FindAsync(id);

        if (todo == null) return false;

        var userId = todo.UserId;
        _context.Todos.Remove(todo);
        await _context.SaveChangesAsync();

        await InvalidateCache(userId, id);
        return true;
    }

    private async Task SaveToCache<T>(string key, T value)
    {
        await _cache.SetStringAsync(
            key,
            JsonSerializer.Serialize(value),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration
            }
        );
    }

    private async Task InvalidateCache(int userId, int? todoId = null)
    {
        await _cache.RemoveAsync("todos:all");
        await _cache.RemoveAsync($"todos:user:{userId}");
        if (todoId.HasValue)
        {
            await _cache.RemoveAsync($"todos:{todoId}");
        }
    }

    private static TodoResponse MapToResponse(Todo todo) => new()
    {
        Id = todo.Id,
        Title = todo.Title,
        Description = todo.Description,
        IsCompleted = todo.IsCompleted,
        CreatedAt = todo.CreatedAt,
        UpdatedAt = todo.UpdatedAt,
        UserId = todo.UserId,
        UserName = todo.User.Name,
    };
}