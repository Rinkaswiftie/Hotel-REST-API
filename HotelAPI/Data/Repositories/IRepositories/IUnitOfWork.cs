namespace HotelAPI.Data.Repositories.IRepositories
{
    public interface IUnitOfWork
    {
        public IHotelRepository HotelRepository { get; }

        public IUserRepository UserRepository { get; }

        Task<int> Save();

    }
}