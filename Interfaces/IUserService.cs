using MyTextApi.Models.DTOs;

namespace MyTextApi.Interfaces;

public interface IUserService
{
    Task<List<UserResponse>> GetAllUsersAsync();
    Task<UserResponse?> GetUserByIdAsync(int id);
    Task<UserResponse> CreateUserAsync(CreateUserRequest request);
    Task<UserResponse?> UpdateUserAsync(int id, CreateUserRequest request);
    Task<bool> DeleteUserAsync(int id);
}