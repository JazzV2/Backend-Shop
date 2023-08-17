using AutoMapper;
using backend.Core.Dtos.AuthUser;
using backend.Core.Dtos.Product;
using backend.Core.Models;

namespace backend.Core.AutoMapperConfig
{
    public class AutoMapperConfigProfile : Profile
    {
        public AutoMapperConfigProfile()
        {
            // AuthUser
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.HashPassword, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));

            // Product
            CreateMap<ProductCreateDto, Product>();
        }
    }
}
