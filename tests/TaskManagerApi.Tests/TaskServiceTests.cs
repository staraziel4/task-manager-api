using Microsoft.EntityFrameworkCore;
using TaskManagerApi.Api.Data;
using TaskManagerApi.Api.DTOs;
using TaskManagerApi.Api.Models;
using TaskManagerApi.Api.Services;
using Xunit;

namespace TaskManagerApi.Tests;

public class TaskServiceTests
{
    private static AppDbContext CreateInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    [Fact]
    public async Task CreateTaskAsync_AddsTaskForCorrectUser()
    {
        await using var db = CreateInMemoryContext();
        var service = new TaskService(db);

        var result = await service.CreateTaskAsync(1, new TaskCreateRequest("Write tests", null, null));

        Assert.Equal("Write tests", result.Title);
        Assert.Equal(TaskItemStatus.Pending, result.Status);
        Assert.Single(db.Tasks);
    }

    [Fact]
    public async Task GetTasksAsync_OnlyReturnsTasksBelongingToUser()
    {
        await using var db = CreateInMemoryContext();
        db.Tasks.AddRange(
            new TaskItem { Title = "User 1 task", UserId = 1 },
            new TaskItem { Title = "User 2 task", UserId = 2 }
        );
        await db.SaveChangesAsync();

        var service = new TaskService(db);
        var result = await service.GetTasksAsync(1, page: 1, pageSize: 20, status: null);

        Assert.Single(result.Items);
        Assert.Equal("User 1 task", result.Items.First().Title);
    }

    [Fact]
    public async Task UpdateTaskAsync_ReturnsNull_WhenTaskBelongsToAnotherUser()
    {
        await using var db = CreateInMemoryContext();
        db.Tasks.Add(new TaskItem { Id = 1, Title = "Original", UserId = 2 });
        await db.SaveChangesAsync();

        var service = new TaskService(db);
        var result = await service.UpdateTaskAsync(1, 1, new TaskUpdateRequest("Hacked", null, TaskItemStatus.Done, null));

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteTaskAsync_RemovesTask_WhenOwnedByUser()
    {
        await using var db = CreateInMemoryContext();
        db.Tasks.Add(new TaskItem { Id = 5, Title = "To delete", UserId = 1 });
        await db.SaveChangesAsync();

        var service = new TaskService(db);
        var deleted = await service.DeleteTaskAsync(1, 5);

        Assert.True(deleted);
        Assert.Empty(db.Tasks);
    }

    [Fact]
    public async Task GetTasksAsync_FiltersByStatus()
    {
        await using var db = CreateInMemoryContext();
        db.Tasks.AddRange(
            new TaskItem { Title = "Pending", UserId = 1, Status = TaskItemStatus.Pending },
            new TaskItem { Title = "Done", UserId = 1, Status = TaskItemStatus.Done }
        );
        await db.SaveChangesAsync();

        var service = new TaskService(db);
        var result = await service.GetTasksAsync(1, page: 1, pageSize: 20, status: TaskItemStatus.Done);

        Assert.Single(result.Items);
        Assert.Equal("Done", result.Items.First().Title);
    }
}
