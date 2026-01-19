using AutoMapper;
using Formation_Ecommerce_11_2025.Application.Products.Dtos;
using Formation_Ecommerce_11_2025.Models.Home;

namespace Formation_Ecommerce_11_2025.Mapping.Home
{
    public class HomeMappingProfile :Profile 
    {
        public HomeMappingProfile()
        {
            CreateMap<HomeProductViewModel, ProductDto>().ReverseMap();
        }
    }
}
