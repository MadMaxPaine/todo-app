using Microsoft.EntityFrameworkCore;
using ToDoBackEnd.Data;
using ToDoBackEnd.Interfaces;
using ToDoBackEnd.Models;

namespace ToDoBackEnd.Data;

public class TaskRepository : ITaskRepository
{
    private readonly AppDbContext _context;

    public TaskRepository(AppDbContext context)
    {
        _context = context;
    }

    
    public IQueryable<TaskItem> Query()
    {
        return _context.Tasks
            .Include(t => t.Category);
    }

    
    public IQueryable<TaskItem> QueryReadOnly()
    {
        return _context.Tasks
            .AsNoTracking()
            .Include(t => t.Category);
    }

    public async Task<TaskItem?> GetById(int userId, int taskId)
    {
        return await _context.Tasks
            .Include(t => t.Category)
            .FirstOrDefaultAsync(t => t.Id == taskId && t.UserId == userId);
    }

    public async Task Add(TaskItem task)
    {
        await _context.Tasks.AddAsync(task);
        await _context.SaveChangesAsync();
    }

    public async Task SaveChanges()
    {
        await _context.SaveChangesAsync();
    }

    public async Task Delete(TaskItem task)
    {
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
    }
}