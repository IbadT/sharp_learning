namespace MyTextApi.Models.Entities;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    // Навигационное свойство для связи с Todo
    public List<Todo> Todos { get; set; } = new();
}