namespace ToDoBackEnd.Dtos;

public class TaskDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }

    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
}