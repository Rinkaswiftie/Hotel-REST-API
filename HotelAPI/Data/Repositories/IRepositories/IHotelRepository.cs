using HotelAPI.Models;

namespace HotelAPI.Data.Repositories.IRepositories
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        public Task<Hotel> Update(Hotel entity);
    }
}