using RecruitmentTasks.Application.DTOs;

namespace RecruitmentTasks.Application.Services;

public interface ICategoryTreeService
{
    Task<List<CategoryDto>> GetCategoryTreeAsync();
}
