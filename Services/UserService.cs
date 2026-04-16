using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MyTextApi.Data;
using MyTextApi.Interfaces;
using MyTextApi.Models.DTOs;
using MyTextApi.Models.Entities;

namespace MyTextApi.Services;

public class UserService : IUserService
{
    private readonly AppDbContext _context;
    private readonly IDistributedCache _cache;
    private readonly TimeSpan _cacheExpiration = TimeSpan.FromMinutes(5);

    public UserService(AppDbContext context, IDistributedCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<UserResponse>> GetAllUsersAsync()
    {
        return await _context.Users
            .Select(u => MapToResponse(u))
            .ToListAsync();
    }

    public async Task<UserResponse?> GetUserByIdAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        return user == null ? null : MapToResponse(user);
    }

    public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
    {
        var user = new User
        {
            Name = request.Name,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return MapToResponse(user);
    }

    public async Task<UserResponse?> UpdateUserAsync(int id, CreateUserRequest request)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return null;

        user.Name = request.Name;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return MapToResponse(user);
    }

    public async Task<bool> DeleteUserAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return false;

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

    private static UserResponse MapToResponse(User user) => new()
    {
        Id = user.Id,
        Name = user.Name,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt,
    };
}