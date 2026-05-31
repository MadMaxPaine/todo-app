using Microsoft.EntityFrameworkCore;
using ToDoBackEnd.Dtos;
using ToDoBackEnd.Interfaces;
using ToDoBackEnd.Models;

namespace ToDoBackEnd.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _repo;

    public TaskService(ITaskRepository repo)
    {
        _repo = repo;
    }

    // MAP ENTITY -> DTO
    private static TaskDto Map(TaskItem task)
    {
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            IsCompleted = task.IsCompleted,
            CreatedAt = task.CreatedAt,
            CategoryId = task.CategoryId,
            CategoryName = task.Category?.Name // IMPORTANT
        };
    }

    public async Task<PagedResult<TaskDto>> GetAll(
        int userId,
        int page,
        int pageSize,
        string? search,
        int? categoryId)
    {
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;

        var query = _repo.Query()
            .Include(t => t.Category) //  FIX: always include category
            .Where(t => t.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(t => t.Title.Contains(search));

        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId);

        var total = await query.CountAsync();

        var items = await query
            .OrderByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<TaskDto>
        {
            Items = items.Select(Map).ToList(),
            TotalCount = total,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<TaskDto?> GetById(int userId, int taskId)
    {
        var task = await _repo.Query()
            .Include(t => t.Category) // CRITICAL FIX
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        return task == null ? null : Map(task);
    }

    public async Task<TaskDto> Create(CreateTaskDto dto, int userId)
    {
        var task = new TaskItem
        {
            Title = dto.Title,
            Description = dto.Description,
            CategoryId = dto.CategoryId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow,
            IsCompleted = false
        };

        await _repo.Add(task);

        // reload WITH category
        var created = await _repo.Query()
            .Include(t => t.Category)
            .FirstAsync(t => t.Id == task.Id);

        return Map(created);
    }

    public async Task<TaskDto?> Update(int userId, int taskId, CreateTaskDto dto)
    {
        var task = await _repo.Query()
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task == null) return null;

        task.Title = dto.Title;
        task.Description = dto.Description;
        task.CategoryId = dto.CategoryId;

        await _repo.SaveChanges();

        // IMPORTANT: return fresh entity with include
        var updated = await _repo.Query()
            .Include(t => t.Category)
            .FirstAsync(t => t.Id == taskId);

        return Map(updated);
    }

    public async Task<bool> Delete(int userId, int taskId)
    {
        var task = await _repo.Query()
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task == null) return false;

        await _repo.Delete(task);
        return true;
    }

    public async Task<TaskDto?> UpdateStatus(int userId, int taskId, bool isCompleted)
    {
        var task = await _repo.Query()
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);

        if (task == null) return null;

        task.IsCompleted = isCompleted;
        await _repo.SaveChanges();

        return Map(task);
    }
}