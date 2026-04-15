using MyTextApi.Interfaces;
using MyTextApi.Models;

namespace MyTextApi.Services;

public class TextService : ITextService
{
    public string GetDefaultText()
    {
        return "Привет! Это дефолтный текст сервера 🚀";
    }

    public string ProcessText(TextRequest request)
    {
        return $"Получен текст: {request.Text}";
    }

    public TextWithIdResponse UpdateText(int id, TextRequest request)
    {
        return new TextWithIdResponse
        {
            Id = id,
            Text = $"Обновленный текст: {request.Text}"
        };
    }

    public DeleteResponse DeleteText(int id)
    {
        return new DeleteResponse
        {
            Id = id,
            Message = $"Элемент с ID {id} удален"
        };
    }
}