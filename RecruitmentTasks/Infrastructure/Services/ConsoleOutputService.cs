namespace RecruitmentTasks.Infrastructure.Services;

using RecruitmentTasks.Common;
using RecruitmentTasks.Application.DTOs;

public interface IConsoleOutputService
{
    void PrintDeveloperBanner();
    void PrintHeader(string title);
    void PrintSeparator();
    void PrintSubSeparator();
    void PrintPerformanceMetrics(string approach, List<long> times, int iterations);
    void PrintComparisonSummary(double efAvg, double sqlAvg);
    void PrintError(string message);
    void DisplayCategoryTree(List<CategoryDto> categories, int indent = 0);
    void DisplayFlatCategoryTree(List<CategoryDto> categories);
}

public class ConsoleOutputService : IConsoleOutputService
{
    public ConsoleOutputService()
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Black;
    }

    public void PrintDeveloperBanner()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string(Constants.Display.SeparatorChar, Constants.Display.SeparatorWidth));
        Console.WriteLine();
        
        Console.ForegroundColor = ConsoleColor.Green;
        var name = "Muhammad Qasim";
        var padding = (Constants.Display.SeparatorWidth - name.Length) / 2;
        Console.WriteLine($"{new string(' ', padding)}{name}");
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        var designation = "Senior Software Engineer | .NET & Angular";
        padding = (Constants.Display.SeparatorWidth - designation.Length) / 2;
        Console.WriteLine($"{new string(' ', padding)}{designation}");
        
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string(Constants.Display.SeparatorChar, Constants.Display.SeparatorWidth));
        Console.ResetColor();
        Console.WriteLine();
    }
    
    public void PrintHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string(Constants.Display.SeparatorChar, Constants.Display.SeparatorWidth));
        
        var padding = (Constants.Display.SeparatorWidth - title.Length) / 2;
        Console.WriteLine($"{new string(' ', padding)}{title}");
        
        Console.WriteLine(new string(Constants.Display.SeparatorChar, Constants.Display.SeparatorWidth));
        Console.ResetColor();
        Console.WriteLine();
    }

    public void PrintSeparator()
    {
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine(new string(Constants.Display.SeparatorChar, Constants.Display.SeparatorWidth));
        Console.ResetColor();
    }

    public void PrintSubSeparator()
    {
        Console.ForegroundColor = ConsoleColor.DarkCyan;
        Console.WriteLine(new string(Constants.Display.SubSeparatorChar, Constants.Display.SeparatorWidth));
        Console.ResetColor();
    }

    public void PrintPerformanceMetrics(string approach, List<long> times, int iterations)
    {
        var avg = times.Average();
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"Performance ({iterations} iterations):");
        Console.ResetColor();
        Console.WriteLine($"  Average: {avg:F2} ms");
        Console.WriteLine($"  Min:     {times.Min()} ms");
        Console.WriteLine($"  Max:     {times.Max()} ms");
    }

    public void PrintComparisonSummary(double efAvg, double sqlAvg)
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("Summary\n");
        Console.ResetColor();

        // Calculate percentage relative to the slower method (baseline)
        var isSqlFaster = sqlAvg < efAvg;
        var performanceGain = isSqlFaster 
            ? (efAvg - sqlAvg) / efAvg * 100      // SQL faster: % improvement over EF
            : (sqlAvg - efAvg) / sqlAvg * 100;     // SQL slower: % slower than EF
        var faster = isSqlFaster ? "faster" : "slower";

        Console.WriteLine($"Entity Framework + LINQ:    {efAvg:F2} ms");
        Console.WriteLine($"T-SQL Stored Procedure:     {sqlAvg:F2} ms");
        Console.WriteLine($"Difference:                 {Math.Abs(efAvg - sqlAvg):F2} ms");
        
        Console.ForegroundColor = isSqlFaster ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"Performance Gain:           {performanceGain:F2}% {faster} with SQL");
        Console.ResetColor();
        Console.WriteLine();
    }

    public void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ERROR] {message}");
        Console.ResetColor();
    }

    public void DisplayCategoryTree(List<CategoryDto> categories, int indent = 0)
    {
        foreach (var category in categories)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{new string(' ', indent * Constants.Display.IndentSpacesPerLevel)}├─ {category.Name} (Level {category.Level})");
            Console.ResetColor();
            
            if (category.Children.Any())
            {
                DisplayCategoryTree([.. category.Children], indent + 1);
            }
        }
    }

    public void DisplayFlatCategoryTree(List<CategoryDto> categories)
    {
        foreach (var category in categories)
        {
            var indent = category.Level * Constants.Display.IndentSpacesPerLevel;
            
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine($"{new string(' ', indent)}├─ {category.Name} (Level {category.Level})");
            Console.ResetColor();
        }
    }
}
