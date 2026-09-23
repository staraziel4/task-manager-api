using TaskManagerApi.Api.DTOs;
using TaskManagerApi.Api.Models;

namespace TaskManagerApi.Api.Services;

public interface ITaskService
{
    Task<PagedResult<TaskResponse>> GetTasksAsync(int userId, int page, int pageSize, TaskItemStatus? status);
    Task<TaskResponse?> GetTaskByIdAsync(int userId, int taskId);
    Task<TaskResponse> CreateTaskAsync(int userId, TaskCreateRequest request);
    Task<TaskResponse?> UpdateTaskAsync(int userId, int taskId, TaskUpdateRequest request);
    Task<bool> DeleteTaskAsync(int userId, int taskId);
}
