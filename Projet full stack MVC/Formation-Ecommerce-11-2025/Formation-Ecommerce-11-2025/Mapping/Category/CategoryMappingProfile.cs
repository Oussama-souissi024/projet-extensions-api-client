using AutoMapper;
using Formation_Ecommerce_11_2025.Application.Categories.Dtos;
using Formation_Ecommerce_11_2025.Models.Category;

namespace Formation_Ecommerce_11_2025.Mapping.Category
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            // Mapping pour CategoryViewModel <-> CategoryDto
            CreateMap<CategoryViewModel, CategoryDto>().ReverseMap();

            CreateMap<DeleteCategoryViewModel, CategoryDto>().ReverseMap();

            CreateMap<CreateCategoryViewModel, CreateCategoryDto>();
            CreateMap<EditCatgoryViewModel, UpdateCategoryDto>().ReverseMap();
        }
    }
}
