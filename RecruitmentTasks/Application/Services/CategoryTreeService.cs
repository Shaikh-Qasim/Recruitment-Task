using Microsoft.EntityFrameworkCore;
using RecruitmentTasks.Persistence;
using RecruitmentTasks.Application.DTOs;
using RecruitmentTasks.Models;

namespace RecruitmentTasks.Application.Services;

public class CategoryTreeService(AppDbContext context) : ICategoryTreeService
{
    public async Task<List<CategoryDto>> GetCategoryTreeAsync()
    {
        var categories = await context.Categories
            .AsNoTracking()
            .ToListAsync();

        var categoryDict = categories.ToDictionary(c => c.Id);
        var rootCategories = new List<CategoryDto>();

        foreach (var category in categories.Where(c => c.ParentId == null).OrderBy(c => c.Name))
        {
            rootCategories.Add(BuildCategoryDto(category, categoryDict));
        }
        
        return rootCategories;
    }

    private CategoryDto BuildCategoryDto(Category category, Dictionary<int, Category> allCategories)
    {
        var children = allCategories.Values
            .Where(c => c.ParentId == category.Id)
            .OrderBy(c => c.Name)
            .Select(child => BuildCategoryDto(child, allCategories))
            .ToList();

        return new CategoryDto
        {
            Id = category.Id,
            Name = category.Name,
            ParentId = category.ParentId,
            Level = category.Level,
            Children = children
        };
    }
}
