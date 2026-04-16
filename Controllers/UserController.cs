using Microsoft.AspNetCore.Mvc;
using MyTextApi.Interfaces;
using MyTextApi.Models.DTOs;

namespace MyTextApi.Controllers;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    // GET /api/users
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // GET /api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        return Ok(user);
    }

    // POST /api/users
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateUserRequest request)
    {
        var user = await _userService.CreateUserAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    // PUT /api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUserRequest request)
    {
        var user = await _userService.UpdateUserAsync(id, request);
        return user == null ? NotFound(new { message = "User not found" }) : Ok(user);
    }

    // DELETE /api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteUserAsync(id);
        return deleted
            ? Ok(new { messssage = $"User {id} deleted successfully" })
            : NotFound(new { message = "User not found" });
    }
}