using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using RecruitmentTasks.Persistence;

namespace RecruitmentTasks.Infrastructure.Services;

public interface IDatabaseInitializationService
{
    Task InitializeAsync();
}

public class DatabaseInitializationService(
    AppDbContext context,
    IConfiguration configuration,
    ILogger<DatabaseInitializationService> logger) : IDatabaseInitializationService
{

    public async Task InitializeAsync()
    {
        logger.LogInformation("Initializing database...");

        try
        {
            var canConnect = await context.Database.CanConnectAsync();

            if (!canConnect)
            {
                logger.LogWarning("Database does not exist or is not accessible. Creating database...");
                await context.Database.EnsureCreatedAsync();
            }

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {count} pending migration(s)", pendingMigrations.Count());
                await context.Database.MigrateAsync();
            }
            else
            {
                logger.LogInformation("Database is up to date");
            }

            var connectionString = configuration.GetConnectionString("Default")
                ?? throw new InvalidOperationException("Connection string 'Default' not found");
            await StoredProcedureInstaller.EnsureAsync(connectionString);

            var count = await context.Categories.CountAsync();
            logger.LogInformation("Database ready with {count} categories", count);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize database");
            throw;
        }
    }
}
