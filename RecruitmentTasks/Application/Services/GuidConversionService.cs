using RecruitmentTasks.Converters;
using RecruitmentTasks.Infrastructure.Services;

namespace RecruitmentTasks.Application.Services;

public interface IGuidConversionService
{
    void Execute();
}

public class GuidConversionService(IConsoleOutputService output) : IGuidConversionService
{

    public void Execute()
    {
        try
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Memory Allocation-Free Base64 URL ↔ GUID Conversion");
            Console.ResetColor();
            output.PrintSubSeparator();
            Console.WriteLine();

            var testGuids = new[]
            {
                Guid.Parse("90a1978c-9f1d-411e-bbe7-079806343eee"),
                Guid.NewGuid(),
                Guid.NewGuid()
            };

            Console.WriteLine("Round-trip conversion tests:\n");

            Span<char> base64UrlBuffer = stackalloc char[22];

            foreach (var originalGuid in testGuids)
            {
                int length = Base64UrlToGuidConverter.TryConvertToBase64Url(originalGuid, base64UrlBuffer);
                string base64Url = new string(base64UrlBuffer.Slice(0, length));

                var reconstructedGuid = Base64UrlToGuidConverter.Convert(base64UrlBuffer.Slice(0, length));
                bool isMatch = originalGuid == reconstructedGuid;

                Console.WriteLine($"Original:      {originalGuid}");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine($"Base64 URL:    {base64Url}");
                Console.ResetColor();
                Console.WriteLine($"Reconstructed: {reconstructedGuid}");
                
                Console.ForegroundColor = isMatch ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine($"Match:         {(isMatch ? "✓" : "✗")}");
                Console.ResetColor();
                Console.WriteLine();
            }

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Features:");
            Console.ResetColor();
            Console.WriteLine("  ✓ Zero heap allocations (Span<T> + stackalloc)");
            Console.WriteLine("  ✓ URL-safe encoding (RFC 4648 Section 5)");
            Console.WriteLine("  ✓ Bidirectional conversion");
            Console.WriteLine("  ✓ AggressiveInlining for optimal performance");
            Console.WriteLine();
        }
        catch (Exception ex)
        {
            output.PrintError($"Task 2 demo failed: {ex.Message}");
            throw;
        }
    }
}
