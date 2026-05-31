using Microsoft.EntityFrameworkCore;
using ToDoBackEnd.Models;

namespace ToDoBackEnd.Data;

public class AppDbContext : DbContext
{
 public AppDbContext(DbContextOptions<AppDbContext> options)
     : base(options) { }

 public DbSet<TaskItem> Tasks => Set<TaskItem>();
 public DbSet<Category> Categories => Set<Category>();
 public DbSet<User> Users { get; set; }

 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
  base.OnModelCreating(modelBuilder);

  modelBuilder.Entity<TaskItem>()
      .HasOne(t => t.User)
      .WithMany(u => u.Tasks)
      .HasForeignKey(t => t.UserId)
      .OnDelete(DeleteBehavior.Cascade);
 }
}