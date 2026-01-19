using AutoMapper;
using Formation_Ecommerce_11_2025.Application.Athentication.Dtos;
using Formation_Ecommerce_11_2025.Models.Auth;

namespace Formation_Ecommerce_11_2025.Mapping.Auth
{
    public class AuthMappingProfile : Profile
    {
        public AuthMappingProfile()
        {
            CreateMap<RegisterViewModel, RegistrationRequestDto>();
            CreateMap<LoginViewModel, LoginRequestDto>();
            CreateMap<ResetPasswordViewModel, ResetPasswordDto>();
        }
    }
}
