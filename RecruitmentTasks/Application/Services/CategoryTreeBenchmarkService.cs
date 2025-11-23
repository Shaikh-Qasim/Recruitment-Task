using System.Diagnostics;
using Microsoft.Extensions.Logging;
using RecruitmentTasks.Infrastructure.Services;
using RecruitmentTasks.Common;

namespace RecruitmentTasks.Application.Services;

public interface ICategoryTreeBenchmarkService
{
    Task ExecuteAsync();
}

public class CategoryTreeBenchmarkService(
    CategoryTreeService efCategoryService,
    SqlCategoryTreeService sqlCategoryService,
    IConsoleOutputService output,
    ILogger<CategoryTreeBenchmarkService> logger) : ICategoryTreeBenchmarkService
{
    public async Task ExecuteAsync()
    {
        try
        {
            Console.WriteLine("APPROACH 1: Entity Framework + LINQ");
            output.PrintSubSeparator();

            var efTimes = await RunEntityFrameworkApproachAsync();
            output.PrintPerformanceMetrics("Entity Framework + LINQ", efTimes, Constants.Benchmark.Iterations);

            Console.WriteLine();
            Console.WriteLine("APPROACH 2: T-SQL Stored Procedure (Recursive CTE)");
            output.PrintSubSeparator();

            var sqlTimes = await RunStoredProcedureApproachAsync();
            output.PrintPerformanceMetrics("T-SQL Stored Procedure", sqlTimes, Constants.Benchmark.Iterations);

            Console.WriteLine();
            var efAvg = efTimes.Average();
            var sqlAvg = sqlTimes.Average();
            
            output.PrintComparisonSummary(efAvg, sqlAvg);
            
            var performanceGain = (efAvg - sqlAvg) / efAvg * 100;
            var faster = sqlAvg < efAvg ? "faster" : "slower";
            logger.LogInformation(
                "Task 1 Complete: EF={efTime}ms, SQL={sqlTime}ms, Gain={gain}% with {faster}",
                efAvg, sqlAvg, Math.Abs(performanceGain), faster);
        }
        catch (Exception ex)
        {
            output.PrintError($"Task 1 comparison failed: {ex.Message}");
            logger.LogError(ex, "Failed to run Task 1 comparison");
            throw;
        }
    }

    private async Task<List<long>> RunEntityFrameworkApproachAsync()
    {
        var times = new List<long>();

        await efCategoryService.GetCategoryTreeAsync();

        for (int i = 0; i < Constants.Benchmark.Iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            var tree = await efCategoryService.GetCategoryTreeAsync();
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);

            if (i == 0)
            {
                Console.WriteLine("Category Tree (First run display):\n");
                output.DisplayCategoryTree(tree);
                Console.WriteLine();
            }
        }

        return times;
    }

    private async Task<List<long>> RunStoredProcedureApproachAsync()
    {
        var times = new List<long>();

        await sqlCategoryService.GetCategoryTreeAsync();

        for (int i = 0; i < Constants.Benchmark.Iterations; i++)
        {
            var sw = Stopwatch.StartNew();
            var tree = await sqlCategoryService.GetCategoryTreeAsync();
            sw.Stop();
            times.Add(sw.ElapsedMilliseconds);

            if (i == 0)
            {
                Console.WriteLine("Category Tree (First run display):\n");
                output.DisplayFlatCategoryTree(tree);
                Console.WriteLine();
            }
        }

        return times;
    }
}
