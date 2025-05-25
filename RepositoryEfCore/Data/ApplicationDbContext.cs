using Microsoft.EntityFrameworkCore;
using RepositoryBase.Models;
using System.Numerics;

namespace RepositoryEfCore.Data;

public class ApplicationDbContext : DbContext
{
    public DbSet<TodoItem> TodoItems { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 생성 날짜 Default 값 세팅
        modelBuilder.Entity<TodoItem>()
        .Property(e => e.CreatedAt)
        .HasDefaultValueSql("GETDATE()");
    }
}
