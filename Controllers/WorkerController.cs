using Microsoft.AspNetCore.Mvc;
using MyTextApi.Interfaces;
using MyTextApi.Models.DTOs;

namespace MyTextApi.Controllers;

[ApiController]
[Route("api/workers")]
public class WorkerController : ControllerBase
{
    private readonly IWorkerService _workerService;
    public WorkerController(IWorkerService workerService)
    {
        _workerService = workerService;
    }

    // GET /api/workers
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var workers = await _workerService.GetAllWorkersAsync();
        return Ok(workers);
    }

    // GET /api/workers/id
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var worker = await _workerService.GetWorkerByIdAsync(id);
        return Ok(worker);
    }

    // GET /api/workers/users/id
    [HttpGet("/users/{id}")]
    public async Task<ActionResult> GetWorkersByUserId(int id)
    {
        var workers = await _workerService.GetWorkersByUserIdAsync(id);
        return Ok(workers);
    }

    // POST /api/workers
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateWorkerRequest request)
    {
        var worker = await _workerService.CreateWorkerAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = worker.Id }, worker);
    }

    // PUT /api/workers/id
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateWorkerRequest request)
    {
        var worker = await _workerService.UpdateWorkerAsync(id, request);
        return worker == null
            ? NotFound(new { message = "Worker not found" })
            : Ok(worker);
    }

    // DELETE /api/workers/id
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _workerService.DeleteWorkerById(id);
        return deleted
            ? Ok(new { message = $"Worker {id} deleted successfully" })
            : NotFound(new { message = "Worker not found" });
    }
}