using HotelAPI.Data.Repositories.IRepositories;
using HotelAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace HotelAPI.Data.Repositories
{
    public class HotelRepository : Repository<Hotel>, IHotelRepository
    {
        private readonly HotelierDBConText _context;
        public HotelRepository(HotelierDBConText context) : base(context)
        {
            _context = context;
        }

        public async Task<Hotel> Update(Hotel entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return entity;
        }

    }
}