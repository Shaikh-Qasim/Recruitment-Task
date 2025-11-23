using AutoMapper;
using RecruitmentTasks.Models;
using RecruitmentTasks.Application.DTOs;

namespace RecruitmentTasks.Application.Mappings;

public class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryDto>();
    }
}
