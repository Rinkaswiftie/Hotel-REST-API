using System.Linq.Expressions;

namespace HotelAPI.Data.Repositories.IRepositories
{
    public interface IRepository<T> where T : class
    {

        public ValueTask<T?> Get(int id);
        public Task<List<T>> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = null
            );
        public Task<T> GetFirstOrDefault(Expression<Func<T, bool>> filter = null,
         string includeProperties = null);
        public void Add(T entity);
        public void Remove(int id);
        public void Remove(T entity);

    }
}