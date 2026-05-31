namespace ToDoBackEnd.Models;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public List<TaskItem> Tasks { get; set; } = new();
}