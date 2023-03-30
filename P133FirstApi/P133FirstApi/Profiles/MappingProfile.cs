using AutoMapper;
using P133FirstApi.DTOs.AuthDTOs;
using P133FirstApi.DTOs.CategorDTOs;
using P133FirstApi.Entities;

namespace P133FirstApi.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CategoryPostDto, Category>()
                .ForMember(des=>des.CreatedAt,src=>src.MapFrom(c=>DateTime.UtcNow.AddHours(4)));

            CreateMap<Category, CategoryGetDto>()
                .ForMember(des => des.EsasKey, src => src.MapFrom(c => c.Id))
                .ForMember(des => des.Adi, src => src.MapFrom(c => c.Name))
                .ForMember(des => des.ProducCou, src => src.MapFrom(c => c.Products.Count()));


            //CreateMap<CategoryGetDto, Category>().ReverseMap();

            CreateMap<RegisterDto, AppUser>();
            //CreateMap<LoginDto, AppUser>();
        }
    }
}
