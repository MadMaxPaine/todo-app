using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ToDoBackEnd.Dtos;
using ToDoBackEnd.Extensions;
using ToDoBackEnd.Interfaces;

namespace ToDoBackEnd.Controllers;

[ApiController]
[Route("api/tasks")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _service;

    public TasksController(ITaskService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        int page = 1,
        int pageSize = 10,
        string? search = null,
        int? categoryId = null)
    {
        var userId = User.GetUserId();

        var result = await _service.GetAll(userId, page, pageSize, search, categoryId);

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();

        var task = await _service.GetById(userId, id);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateTaskDto dto)
    {
        var userId = User.GetUserId();

        var task = await _service.Create(dto, userId);

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            task
        );
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateTaskDto dto)
    {
        var userId = User.GetUserId();

        var task = await _service.Update(userId, id, dto);

        if (task == null)
            return NotFound();

        return Ok(task);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();

        var result = await _service.Delete(userId, id);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpPatch("{id}")]
    public async Task<IActionResult> UpdateStatus(int id, UpdateTaskStatusDto dto)
    {
        var userId = User.GetUserId();

        var task = await _service.UpdateStatus(userId, id, dto.IsCompleted);

        if (task == null)
            return NotFound();

        return Ok(task);
    }
}