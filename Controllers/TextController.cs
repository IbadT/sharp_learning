using Microsoft.AspNetCore.Mvc;
using MyTextApi.Interfaces;
using MyTextApi.Models;

namespace MyTextApi.Controllers;

[ApiController]
// [Route("api/[controller]")] // Берет имя контроллера (без "Controller") для маршрутизации - будет api/Text
[Route("api/v1")]
public class TextController : ControllerBase
{
    public readonly ITextService _textService;

    // Dependency Injection через конструктор
    public TextController(ITextService textService)
    {
        _textService = textService;
    }

    // GET: api/text
    [HttpGet]
    public IActionResult Get()
    {
        var result = _textService.GetDefaultText();
        return Ok(result);
    }

    // POST: api/text
    [HttpPost]
    public IActionResult Post([FromBody] TextRequest request)
    {
        var result = _textService.ProcessText(request);
        return Ok(result);
    }

    // PUT: api/text/5
    [HttpPut("{id}")]
    public IActionResult Put(int id, [FromBody] TextRequest request)
    {
        var result = _textService.UpdateText(id, request);
        return Ok(result);
    }

    // DELETE: api/text/5
    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var result = _textService.DeleteText(id);
        return Ok(result);
    }
}