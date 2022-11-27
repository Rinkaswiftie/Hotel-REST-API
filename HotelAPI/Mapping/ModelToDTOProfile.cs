using AutoMapper;
using HotelAPI.Models;
using HotelAPI.Models.DTO;

namespace HotelAPI.Mapping
{
    public class ModelToDTOProfile: Profile
    {
        public ModelToDTOProfile()
        {
            CreateMap<Hotel, HotelDTO>();

            CreateMap<User, NewUserResponse>();
        }
    }
}