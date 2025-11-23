namespace RecruitmentTasks.Application.DTOs;

public sealed class CategoryDto : ICategoryDisplay
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public int? ParentId { get; init; }
    public int Level { get; init; }
    public CategoryDto? Parent { get; init; }
    public IReadOnlyCollection<CategoryDto> Children { get; init; } = Array.Empty<CategoryDto>();
}
