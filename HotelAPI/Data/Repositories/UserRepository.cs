using HotelAPI.Data.Repositories.IRepositories;
using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private readonly HotelierDBConText _context;
        public UserRepository(HotelierDBConText context) : base(context)
        {
            _context = context;
        }

        public async Task<User> FindByEmailAsync(string email)
        {            
            return await _context.Users.FirstOrDefaultAsync(r => r.Email.Equals(email));
        }

    }
}