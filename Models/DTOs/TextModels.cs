namespace MyTextApi.Models;

// Для POST и PUT запросов (текст из body)
public class TextRequest
{
    public string Text { get; set; } = string.Empty;
}

// Для PUT ответа (текст + id)
public class TextWithIdResponse
{
    public string Text { get; set; } = string.Empty;
    public int Id { get; set; }
}

// для DELETE ответа
public class DeleteResponse
{
    public int Id { get; set; }
    public string Message { get; set; } = string.Empty;
}