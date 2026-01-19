using AutoMapper;
using Formation_Ecommerce_11_2025.Application.Cart.Dtos;
using Formation_Ecommerce_11_2025.Application.Orders.Dtos;
using Formation_Ecommerce_11_2025.Models.Cart;

namespace Formation_Ecommerce_11_2025.Mapping.Cart
{
    public class CartMappingProfile : Profile
    {
        public CartMappingProfile()
        {
            CreateMap<CartDto, CartIndexViewModel>().ReverseMap();
            CreateMap<CartDto, CheckoutCartViewModel>().ReverseMap();
            CreateMap<CartHeaderDto, OrderHeaderDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<CartDetailsDto, OrderDetailsDto>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Product != null ? src.Product.Price : (src.Price ?? 0m)));
        }
    }
}
