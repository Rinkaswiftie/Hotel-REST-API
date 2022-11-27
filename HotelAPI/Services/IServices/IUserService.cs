using HotelAPI.Models;
using HotelAPI.Models.DTO;

namespace HotelAPI.Services.iServices
{

    public interface IUserService
    {
        
        public Task<User> GetById(int id);

        public Task<User> CreateANewUser(User user);

        public Boolean verifyPasswords(User user, UserCredentials credentials);

        public Task<bool> UserExists(string email);
    }
}