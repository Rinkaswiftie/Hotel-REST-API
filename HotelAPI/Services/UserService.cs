using HotelAPI.Data.Repositories.IRepositories;
using HotelAPI.Models;
using HotelAPI.Models.DTO;
using HotelAPI.Services.iServices;

namespace HotelAPI.Services
{
    public class UserService : IUserService
    {

        private readonly IUnitOfWork _db;

        public UserService(IUnitOfWork db)
        {
            _db = db;
        }

        public async Task<User> GetById(int id)
        {
            return await _db.UserRepository.Get(id);
        }

        public async Task<User> CreateANewUser(User user)
        {
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.UserRoles = new List<Role>{
                Role.User
            };
            _db.UserRepository.Add(user);
            await _db.Save();
            return await _db.UserRepository.FindByEmailAsync(user.Email);
        }

        public Boolean verifyPasswords(User user, UserCredentials credentials)
        {
            return BCrypt.Net.BCrypt.Verify(credentials.Password, user.Password);
        }

        public async Task<bool> UserExists(string email)
        {
            var userExists = await _db.UserRepository.FindByEmailAsync(email);
            return userExists != null;
        }


    }
}