using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ToDoBackEnd.Data;
using ToDoBackEnd.Models;
using ToDoBackEnd.Dtos;

namespace ToDoBackEnd.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    // GET: api/categories
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var categories = await _context.Categories
            .Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name
            })
            .ToListAsync();

        return Ok(categories);
    }

    // GET: api/categories/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        return Ok(new CategoryDto
        {
            Id = category.Id,
            Name = category.Name
        });
    }

    // POST: api/categories
    [HttpPost]
    public async Task<IActionResult> Create(CategoryDto dto)
    {
        var category = new Category
        {
            Name = dto.Name
        };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync();

        dto.Id = category.Id;
        return Ok(dto);
    }

    // DELETE: api/categories/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();

        return Ok();
    }
}