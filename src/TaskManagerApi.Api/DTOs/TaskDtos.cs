using System.ComponentModel.DataAnnotations;
using TaskManagerApi.Api.Models;

namespace TaskManagerApi.Api.DTOs;

public record TaskCreateRequest(
    [Required, MaxLength(200)] string Title,
    string? Description,
    DateTime? DueDate
);

public record TaskUpdateRequest(
    [Required, MaxLength(200)] string Title,
    string? Description,
    TaskItemStatus Status,
    DateTime? DueDate
);

public record TaskResponse(
    int Id,
    string Title,
    string? Description,
    TaskItemStatus Status,
    DateTime CreatedAt,
    DateTime? DueDate
);

public record PagedResult<T>(IEnumerable<T> Items, int Page, int PageSize, int TotalCount);
