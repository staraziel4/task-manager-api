using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Api.Data;
using TaskManagerApi.Api.DTOs;
using TaskManagerApi.Api.Models;

namespace TaskManagerApi.Api.Services;

public class TaskService : ITaskService
{
    private readonly AppDbContext _db;

    public TaskService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PagedResult<TaskResponse>> GetTasksAsync(int userId, int page, int pageSize, TaskItemStatus? status)
    {
        page = page < 1 ? 1 : page;
        pageSize = pageSize is < 1 or > 100 ? 20 : pageSize;

        var query = _db.Tasks.Where(t => t.UserId == userId);

        if (status is not null)
            query = query.Where(t => t.Status == status);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => ToResponse(t))
            .ToListAsync();

        return new PagedResult<TaskResponse>(items, page, pageSize, totalCount);
    }

    public async Task<TaskResponse?> GetTaskByIdAsync(int userId, int taskId)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
        return task is null ? null : ToResponse(task);
    }

    public async Task<TaskResponse> CreateTaskAsync(int userId, TaskCreateRequest request)
    {
        var task = new TaskItem
        {
            Title = request.Title,
            Description = request.Description,
            DueDate = request.DueDate,
            UserId = userId
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        return ToResponse(task);
    }

    public async Task<TaskResponse?> UpdateTaskAsync(int userId, int taskId, TaskUpdateRequest request)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
        if (task is null)
            return null;

        task.Title = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.DueDate = request.DueDate;

        await _db.SaveChangesAsync();
        return ToResponse(task);
    }

    public async Task<bool> DeleteTaskAsync(int userId, int taskId)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
        if (task is null)
            return false;

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return true;
    }

    private static TaskResponse ToResponse(TaskItem t) =>
        new(t.Id, t.Title, t.Description, t.Status, t.CreatedAt, t.DueDate);
}
