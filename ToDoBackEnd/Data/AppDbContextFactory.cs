using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ToDoBackEnd.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

        optionsBuilder.UseSqlServer(
            "Server=localhost,1433;Database=TodoDb;User Id=sa;Password=StrongPassword123;TrustServerCertificate=True;"
        );

        return new AppDbContext(optionsBuilder.Options);
    }
}