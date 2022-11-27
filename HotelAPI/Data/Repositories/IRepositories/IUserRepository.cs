using HotelAPI.Models;

namespace HotelAPI.Data.Repositories.IRepositories
{
    public interface IUserRepository : IRepository<User>
    {
        public Task<User> FindByEmailAsync(string email);
    }
}