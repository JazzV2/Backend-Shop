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
            CreateMap<Product, ProductGetDto>()
                .ForMember(dest => dest.FirstImage, opt => opt.MapFrom(src => Convert.ToBase64String(src.Images.FirstOrDefault(image => image.IsMain).ProductImage)));
            CreateMap<Product, ProductByUrlGetDto>();
        }
    }
}
