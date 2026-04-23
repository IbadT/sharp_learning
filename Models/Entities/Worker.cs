namespace MyTextApi.Models.Entities;

public class Worker
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Связь с User
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}