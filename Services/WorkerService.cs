using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyTextApi.Data;
using MyTextApi.Interfaces;
using MyTextApi.Models.DTOs;
using MyTextApi.Models.Entities;

namespace MyTextApi.Services;

public class WorkerService : IWorkerService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);
    public WorkerService(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<WorkerResponse> CreateWorkerAsync(CreateWorkerRequest request)
    {
        var worker = new Worker
        {
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            UserId = request.UserId,
        };

        _context.Workers.Add(worker);
        await _context.SaveChangesAsync();

        // Инвалидируем кэш списка
        await _cache.RemoveAsync("workers:all");

        return MapToResponse(worker);
    }

    public async Task<List<WorkerResponse>> GetAllWorkersAsync()
    {
        var cacheKey = "workers";
        var cached = await _cache.GetStringAsync(cacheKey);

        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<List<WorkerResponse>>(cached)!;
        }

        var workers = await _context.Workers
            .Include(w => w.User)
            .Select(w => MapToResponse(w))
            .ToListAsync();

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(workers),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration,
            }
        );

        return workers;
    }

    public async Task<WorkerResponse?> GetWorkerByIdAsync(int id)
    {
        var cacheKey = $"workers:{id}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<WorkerResponse>(cached);
        }

        var worker = await _context.Workers.FindAsync(id);
        if (worker == null) return null;

        var response = MapToResponse(worker);

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(response),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration,
            }
        );

        return response;
    }

    public async Task<List<WorkerResponse>?> GetWorkersByUserIdAsync(int userId)
    {
        var cacheKey = $"workers:{userId}";

        var cached = await _cache.GetStringAsync(cacheKey);
        if (!string.IsNullOrEmpty(cached))
        {
            return JsonSerializer.Deserialize<List<WorkerResponse>>(cached);
        }
        var workers = await _context.Workers
            .Include(w => w.User)
            .Where(w => w.UserId == userId)
            .Select(w => MapToResponse(w))
            .ToListAsync();

        await _cache.SetStringAsync(
            cacheKey,
            JsonSerializer.Serialize(workers),
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = _cacheExpiration,
            }
        );

        return workers;
    }

    public async Task<WorkerResponse?> UpdateWorkerAsync(int id, CreateWorkerRequest request)
    {
        var worker = await _context.Workers.FindAsync(id);
        if (worker == null) return null;

        worker.Email = request.Email;
        worker.Name = request.Name;
        worker.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // Инвалидируем кэш списка
        await _cache.RemoveAsync($"workers:{id}");
        await _cache.RemoveAsync("workers:all");

        return MapToResponse(worker);
    }

    public async Task<bool> DeleteWorkerById(int id)
    {
        var worker = await _context.Workers.FindAsync(id);
        if (worker == null) return false;

        _context.Workers.Remove(worker);
        await _context.SaveChangesAsync();

        // Инвалидируем кэш списка
        await _cache.RemoveAsync($"workers:{id}");
        await _cache.RemoveAsync("workers:all");

        return true;
    }

    private static WorkerResponse MapToResponse(Worker worker) => new()
    {
        Id = worker.Id,
        Name = worker.Name,
        Email = worker.Email,
        CreatedAt = worker.CreatedAt,
        UpdatedAt = worker.UpdatedAt,
        UserId = worker.UserId,
    };
}