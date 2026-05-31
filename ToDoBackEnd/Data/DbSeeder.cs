using Microsoft.EntityFrameworkCore;
using ToDoBackEnd.Models;

namespace ToDoBackEnd.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.MigrateAsync();

        // =========================
        // USERS
        // =========================
        if (!await context.Users.AnyAsync())
        {
            var user = new User
            {
                Email = "test@test.com",
                PasswordHash = "test"
            };

            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();
        }

        var userId = await context.Users.Select(x => x.Id).FirstAsync();

        // =========================
        // CATEGORIES
        // =========================
        if (!await context.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new Category { Name = "Work" },
                new Category { Name = "Home" },
                new Category { Name = "Study" },
                new Category { Name = "Health" },
                new Category { Name = "Fitness" },
                new Category { Name = "Finance" },
                new Category { Name = "Shopping" },
                new Category { Name = "Personal" },
                new Category { Name = "Travel" },
                new Category { Name = "Projects" },
                new Category { Name = "Ideas" }
            };

            await context.Categories.AddRangeAsync(categories);
            await context.SaveChangesAsync();
        }

        var categoriesDb = await context.Categories.ToListAsync();

        // =========================
        // TASKS
        // =========================
        if (!await context.Tasks.AnyAsync())
        {
            var tasks = new List<TaskItem>
            {
                new TaskItem
                {
                    Title = "Fix API bugs",
                    Description = "Clean up backend issues",
                    IsCompleted = false,
                    CategoryId = categoriesDb[0].Id,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                },
                new TaskItem
                {
                    Title = "Buy groceries",
                    Description = "Milk, eggs, bread",
                    IsCompleted = false,
                    CategoryId = categoriesDb[1].Id,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                },
                new TaskItem
                {
                    Title = "Study EF Core",
                    Description = "Learn migrations and relations",
                    IsCompleted = true,
                    CategoryId = categoriesDb[2].Id,
                    UserId = userId,
                    CreatedAt = DateTime.UtcNow
                }
            };

            await context.Tasks.AddRangeAsync(tasks);
            await context.SaveChangesAsync();
        }
    }
}