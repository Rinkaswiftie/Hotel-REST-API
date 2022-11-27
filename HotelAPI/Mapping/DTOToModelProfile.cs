using AutoMapper;
using HotelAPI.Models;
using HotelAPI.Models.DTO;

namespace HotelAPI.Mapping
{
    public class DTOToModelProfile : Profile
    {
        public DTOToModelProfile()
        {
            CreateMap<HotelDTO, Hotel>();
            CreateMap<UserCredentials, User>();
            CreateMap<SignUpRequest, User>();
        }
    }
}