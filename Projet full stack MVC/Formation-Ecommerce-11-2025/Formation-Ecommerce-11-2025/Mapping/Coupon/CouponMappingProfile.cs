using AutoMapper;
using Formation_Ecommerce_11_2025.Application.Coupons.Dtos;
using Formation_Ecommerce_11_2025.Models.Coupon;

namespace Formation_Ecommerce_11_2025.Mapping.Coupon
{
    public class CouponMappingProfile : Profile
    {
        public CouponMappingProfile()
        {
            CreateMap<CouponDto, CouponViewModel>().ReverseMap();
            CreateMap<CreateCouponViewModel, CouponDto>().ReverseMap();
            CreateMap<DeleteCouponViewModel, CouponDto>().ReverseMap();
        }
    }
}
