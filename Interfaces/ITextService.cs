using MyTextApi.Models;

namespace MyTextApi.Interfaces;

public interface ITextService
{
    string GetDefaultText();
    string ProcessText(TextRequest request);
    TextWithIdResponse UpdateText(int id, TextRequest request);
    DeleteResponse DeleteText(int id);
}