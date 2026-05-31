namespace ToDoBackEnd.Dtos;

public class CreateTaskDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
}