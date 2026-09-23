using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerApi.Api.DTOs;
using TaskManagerApi.Api.Models;
using TaskManagerApi.Api.Services;

namespace TaskManagerApi.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
    }

    private int CurrentUserId =>
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirstValue("sub")!);

    [HttpGet]
    public async Task<ActionResult<PagedResult<TaskResponse>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] TaskItemStatus? status = null)
    {
        var result = await _taskService.GetTasksAsync(CurrentUserId, page, pageSize, status);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskResponse>> GetById(int id)
    {
        var task = await _taskService.GetTaskByIdAsync(CurrentUserId, id);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpPost]
    public async Task<ActionResult<TaskResponse>> Create(TaskCreateRequest request)
    {
        var task = await _taskService.CreateTaskAsync(CurrentUserId, request);
        return CreatedAtAction(nameof(GetById), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TaskResponse>> Update(int id, TaskUpdateRequest request)
    {
        var task = await _taskService.UpdateTaskAsync(CurrentUserId, id, request);
        return task is null ? NotFound() : Ok(task);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _taskService.DeleteTaskAsync(CurrentUserId, id);
        return deleted ? NoContent() : NotFound();
    }
}
