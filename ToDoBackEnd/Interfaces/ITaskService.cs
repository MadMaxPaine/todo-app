using ToDoBackEnd.Dtos;

namespace ToDoBackEnd.Interfaces;

public interface ITaskService
{
    Task<PagedResult<TaskDto>> GetAll(
        int userId,
        int page,
        int pageSize,
        string? search,
        int? categoryId);

    Task<TaskDto?> GetById(int userId, int taskId);

    Task<TaskDto> Create(CreateTaskDto dto, int userId);

    Task<TaskDto?> Update(int userId, int taskId, CreateTaskDto dto);
    Task<TaskDto?> UpdateStatus(int userId, int taskId, bool isCompleted);
    Task<bool> Delete(int userId, int taskId);
}