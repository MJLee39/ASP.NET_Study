using Microsoft.EntityFrameworkCore;
using SimpleNET.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }
    //데이터베이스 테이블에 매핑될 DbSet 정의
    public DbSet<Book> Books{get; set;}
    public DbSet<Rental> Rentals{get; set;}
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=localhost,1433;Database=master;User Id=SA;Password=YourPassword123;TrustServerCertificate=True;Encrypt=False;");
        optionsBuilder.EnableSensitiveDataLogging();
        optionsBuilder.LogTo(Console.WriteLine);
    }
}