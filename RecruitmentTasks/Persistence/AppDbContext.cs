using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RecruitmentTasks.Models;
using RecruitmentTasks.Persistence.Configuration;

namespace RecruitmentTasks.Persistence;

public class AppDbContext : DbContext
{
    public DbSet<Category> Categories { get; set; }

    public AppDbContext() { }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: true)
                .Build();
            var cs = configuration.GetConnectionString("Default") ?? configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrWhiteSpace(cs))
                optionsBuilder.UseSqlServer(cs);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Level).IsRequired();
            
            entity.HasIndex(e => e.ParentId);

            // Seed data from JSON file
            var seedData = CategorySeedConfiguration.GetSeedData();
            entity.HasData(seedData);
        });
    }
}
