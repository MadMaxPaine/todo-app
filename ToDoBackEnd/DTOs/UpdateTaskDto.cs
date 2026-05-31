namespace ToDoBackEnd.Dtos;

public class UpdateTaskDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public int CategoryId { get; set; }
}