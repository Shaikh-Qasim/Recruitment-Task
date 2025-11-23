using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AutoMapper;
using RecruitmentTasks.Persistence;
using RecruitmentTasks.Application.Services;
using RecruitmentTasks.Infrastructure.Services;
using RecruitmentTasks.Application.Mappings;

using var serviceProvider = ConfigureServices();

try
{
    var output = serviceProvider.GetRequiredService<IConsoleOutputService>();
    
    output.PrintDeveloperBanner();
    output.PrintHeader("CATEGORY TREE PERFORMANCE COMPARISON");

    using (var scope = serviceProvider.CreateScope())
    {
        var dbInit = scope.ServiceProvider.GetRequiredService<IDatabaseInitializationService>();
        await dbInit.InitializeAsync();
    }

    output.PrintSeparator();
    Console.WriteLine("Task 1: Category Tree Performance Comparison\n");
    
    using (var scope = serviceProvider.CreateScope())
    {
        var benchmark = scope.ServiceProvider.GetRequiredService<ICategoryTreeBenchmarkService>();
        await benchmark.ExecuteAsync();
    }

    output.PrintSeparator();
    Console.WriteLine("Task 2: Memory Allocation-Free Base64 URL to GUID Converter\n");
    
    var guidConverter = serviceProvider.GetRequiredService<IGuidConversionService>();
    guidConverter.Execute();

    output.PrintSeparator();
    Console.WriteLine("\nPress any key to exit...");
    Console.ReadKey();

    return 0;
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[ERROR] {ex.Message}");
    Console.ResetColor();
    return 1;
}

static ServiceProvider ConfigureServices()
{
    var services = new ServiceCollection();
    
    var configuration = new ConfigurationBuilder()
        .SetBasePath(AppContext.BaseDirectory)
        .AddJsonFile("appsettings.json", optional: false)
        .Build();

    services.AddSingleton<IConfiguration>(configuration);
    
    var connectionString = configuration.GetConnectionString("Default")
        ?? throw new InvalidOperationException("Connection string 'Default' not found in configuration");
    
    services.AddAutoMapper(typeof(CategoryProfile).Assembly);
    
    services.AddLogging(builder =>
    {
        builder.ClearProviders();
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Warning);
    });

    services.AddDbContext<AppDbContext>(options =>
    {
        options.UseSqlServer(connectionString);
    });
    
    services.AddSingleton<IConsoleOutputService, ConsoleOutputService>();
    services.AddSingleton<IGuidConversionService, GuidConversionService>();
    
    services.AddScoped<IDatabaseInitializationService, DatabaseInitializationService>();
    
    services.AddScoped<CategoryTreeService>();
    services.AddScoped<SqlCategoryTreeService>(_ => new SqlCategoryTreeService(connectionString));
    
    services.AddScoped<ICategoryTreeBenchmarkService>(sp =>
    {
        var efService = sp.GetRequiredService<CategoryTreeService>();
        var sqlService = sp.GetRequiredService<SqlCategoryTreeService>();
        var output = sp.GetRequiredService<IConsoleOutputService>();
        var logger = sp.GetRequiredService<ILogger<CategoryTreeBenchmarkService>>();
        return new CategoryTreeBenchmarkService(efService, sqlService, output, logger);
    });

    return services.BuildServiceProvider();
}
