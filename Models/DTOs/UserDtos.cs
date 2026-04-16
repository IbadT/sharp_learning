namespace MyTextApi.Models.DTOs;

// POST / PUT
public class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;
}

// GET response
public class UserResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}