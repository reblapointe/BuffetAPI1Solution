using AutoMapper;
using BuffetAPI.Data;
using BuffetAPI.Models.Plats;

namespace BuffetAPI.Configurations
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Plat, PostPlatDto>().ReverseMap();
            CreateMap<Plat, PutPlatDto>().ReverseMap();
        }
    }
}