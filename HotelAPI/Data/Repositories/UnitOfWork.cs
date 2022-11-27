using HotelAPI.Data.Repositories.IRepositories;

namespace HotelAPI.Data.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly HotelierDBConText _context;
        public IHotelRepository HotelRepository { get; private set; }
        public IUserRepository UserRepository { get; private set; }

        public UnitOfWork(HotelierDBConText context)
        {
            _context = context;
            HotelRepository = new HotelRepository(context);
            UserRepository = new UserRepository(context);
        }

        public ValueTask Dispose()
        {
            return _context.DisposeAsync();
        }

        public Task<int> Save()
        {
            return _context.SaveChangesAsync();
        }

    }
}