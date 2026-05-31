using ToDoBackEnd.Models;

namespace ToDoBackEnd.Interfaces;

public interface ITaskRepository
{
    IQueryable<TaskItem> Query();

    Task<TaskItem?> GetById(int userId, int taskId);

    Task Add(TaskItem task);

    Task SaveChanges();

    Task Delete(TaskItem task);
}