using AutoMapper;
using FirstAPIConnectDB.DTOs;
using FirstAPIConnectDB.Models;

namespace FirstAPIConnectDB.MappingConfigs
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AuthorAddDTO, Author>();
            CreateMap<Author, AuthorAddDTO>();
            CreateMap<AuthorDTO, Author>().ReverseMap();
        }
    }
}
